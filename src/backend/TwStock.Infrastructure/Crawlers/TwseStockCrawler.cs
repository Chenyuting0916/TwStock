using System.Text.Json;
using TwStock.Application.Interfaces;
using TwStock.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace TwStock.Infrastructure.Crawlers;

/// <summary>
/// Fetches stock list from FinMind TaiwanStockInfo dataset
/// This includes industry classification which TWSE Open API doesn't provide
/// </summary>
public class TwseStockCrawler : IStockCrawler
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TwseStockCrawler> _logger;

    private const string FinMindBaseUrl = "https://api.finmindtrade.com/api/v4/data";

    public TwseStockCrawler(HttpClient httpClient, ILogger<TwseStockCrawler> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Stock>> FetchStockListAsync()
    {
        var stocks = new List<Stock>();

        try
        {
            _logger.LogInformation("Fetching Taiwan stock list from FinMind TaiwanStockInfo...");

            // Fetch from FinMind which includes industry classification
            var url = $"{FinMindBaseUrl}?dataset=TaiwanStockInfo";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"FinMind API returned {response.StatusCode}");
                return stocks;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var dataArray = doc.RootElement.GetProperty("data");

            foreach (var item in dataArray.EnumerateArray())
            {
                try
                {
                    // FinMind TaiwanStockInfo fields: stock_id, stock_name, industry_category, type, date
                    if (!item.TryGetProperty("stock_id", out var stockIdElem))
                        continue;

                    var symbol = stockIdElem.GetString() ?? "";

                    // Filter - only keep stocks (4 digits), not warrants or other securities
                    if (string.IsNullOrWhiteSpace(symbol) || symbol.Length != 4)
                        continue;

                    if (!item.TryGetProperty("stock_name", out var nameElem))
                        continue;

                    var name = nameElem.GetString() ?? "";

                    // Get industry category
                    var industry = "未分類";
                    if (item.TryGetProperty("industry_category", out var industryElem))
                    {
                        var industryValue = industryElem.GetString();
                        if (!string.IsNullOrWhiteSpace(industryValue))
                        {
                            industry = industryValue;
                        }
                    }

                    // Get market type
                    var market = "TSE"; // Default to TSE
                    if (item.TryGetProperty("type", out var typeElem))
                    {
                        var typeValue = typeElem.GetString() ?? "";
                        if (typeValue.Contains("上櫃") || typeValue.Contains("OTC"))
                        {
                            market = "OTC";
                        }
                        else if (typeValue.Contains("興櫃"))
                        {
                            market = "ESB"; // Emerging Stock Board
                        }
                    }

                    stocks.Add(new Stock
                    {
                        Symbol = symbol,
                        Name = name,
                        Industry = industry,
                        Market = market
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Error parsing stock item");
                    continue;
                }
            }

            _logger.LogInformation($"Successfully fetched {stocks.Count} stocks with industry classification");

            return stocks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock list");
            return new List<Stock>();
        }
    }
}
