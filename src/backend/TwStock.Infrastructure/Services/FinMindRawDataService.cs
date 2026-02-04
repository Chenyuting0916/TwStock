using TwStock.Domain.Entities;
using TwStock.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TwStock.Infrastructure.Services;

/// <summary>
/// Service for fetching and storing raw FinMind API data
/// </summary>
public class FinMindRawDataService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ILogger<FinMindRawDataService> _logger;

    private const string FinMindBaseUrl = "https://api.finmindtrade.com/api/v4/data";

    // FinMind datasets to fetch
    private static readonly string[] Datasets = new[]
    {
        "TaiwanStockFinancialStatements",  // 損益表
        "TaiwanStockBalanceSheet",          // 資產負債表
        "TaiwanStockCashFlowsStatement",    // 現金流量表
        "TaiwanStockDividend"               // 股利政策
    };

    public FinMindRawDataService(
        AppDbContext context,
        HttpClient httpClient,
        ILogger<FinMindRawDataService> logger)
    {
        _context = context;
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Fetch and store raw data for a single stock
    /// </summary>
    public async Task FetchAndStoreRawDataAsync(string symbol)
    {
        _logger.LogInformation($"Starting raw data fetch for {symbol}");

        var stock = _context.Stocks.FirstOrDefault(s => s.Symbol == symbol);
        if (stock == null)
        {
            _logger.LogWarning($"Stock {symbol} not found in database");
            return;
        }

        var endDate = DateTime.Now.ToString("yyyy-MM-dd");
        var startDate = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd");

        foreach (var dataset in Datasets)
        {
            try
            {
                _logger.LogInformation($"Fetching {dataset} for {symbol}");

                var url = $"{FinMindBaseUrl}?dataset={dataset}&data_id={symbol}&start_date={startDate}&end_date={endDate}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"FinMind API returned {response.StatusCode} for {dataset}");
                    continue;
                }

                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);

                var dataArray = doc.RootElement.GetProperty("data");
                var rawDataList = new List<FinMindRawData>();

                foreach (var item in dataArray.EnumerateArray())
                {
                    try
                    {
                        // Parse date
                        if (!item.TryGetProperty("date", out var dateElem))
                            continue;

                        var dateStr = dateElem.GetString();
                        if (!DateTime.TryParse(dateStr, out var date))
                            continue;

                        // For dividend dataset, structure is different
                        if (dataset == "TaiwanStockDividend")
                        {
                            // Store dividend data as individual records
                            if (item.TryGetProperty("CashEarningsDistribution", out var cashDiv))
                            {
                                rawDataList.Add(CreateRawData(stock.Id, dataset, date, "CashEarningsDistribution",
                                    GetDecimalValue(cashDiv), "現金股利"));
                            }

                            if (item.TryGetProperty("StockEarningsDistribution", out var stockDiv))
                            {
                                rawDataList.Add(CreateRawData(stock.Id, dataset, date, "StockEarningsDistribution",
                                    GetDecimalValue(stockDiv), "股票股利"));
                            }
                        }
                        else
                        {
                            // For financial statements and balance sheet
                            if (!item.TryGetProperty("type", out var typeElem) ||
                                !item.TryGetProperty("value", out var valueElem))
                                continue;

                            var type = typeElem.GetString() ?? "";
                            var value = GetDecimalValue(valueElem);

                            string? originName = null;
                            if (item.TryGetProperty("origin_name", out var originElem))
                            {
                                originName = originElem.GetString();
                            }

                            rawDataList.Add(CreateRawData(stock.Id, dataset, date, type, value, originName));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Error parsing individual data item");
                        continue;
                    }
                }

                // Remove existing data for this stock/dataset combination
                var existing = _context.FinMindRawData
                    .Where(r => r.StockId == stock.Id && r.Dataset == dataset)
                    .ToList();

                _context.FinMindRawData.RemoveRange(existing);

                // Add new data
                _context.FinMindRawData.AddRange(rawDataList);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Stored {rawDataList.Count} raw records for {symbol} - {dataset}");

                // Rate limiting
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching {dataset} for {symbol}");
            }
        }

        _logger.LogInformation($"Completed raw data fetch for {symbol}");
    }

    /// <summary>
    /// Fetch and store raw data for all stocks
    /// </summary>
    public async Task FetchAndStoreAllRawDataAsync()
    {
        var stocks = _context.Stocks.Select(s => s.Symbol).ToList();
        _logger.LogInformation($"Starting raw data fetch for {stocks.Count} stocks");

        var count = 0;
        foreach (var symbol in stocks)
        {
            count++;
            _logger.LogInformation($"Processing {count}/{stocks.Count}: {symbol}");

            await FetchAndStoreRawDataAsync(symbol);

            // Rate limiting between stocks
            await Task.Delay(1000);
        }

        _logger.LogInformation("Completed raw data fetch for all stocks");
    }

    private FinMindRawData CreateRawData(int stockId, string dataset, DateTime date,
        string type, decimal value, string? originName)
    {
        return new FinMindRawData
        {
            StockId = stockId,
            Dataset = dataset,
            Date = date,
            Type = type,
            Value = value,
            OriginName = originName,
            CreatedDate = DateTime.UtcNow
        };
    }

    private decimal GetDecimalValue(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Number)
        {
            return element.GetDecimal();
        }
        return 0m;
    }
}
