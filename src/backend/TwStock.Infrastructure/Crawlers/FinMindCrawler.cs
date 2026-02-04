using System.Text.Json;
using TwStock.Application.Interfaces;
using TwStock.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace TwStock.Infrastructure.Crawlers;

/// <summary>
/// Complete FinMind API Crawler with quarterly data and TTM support
/// </summary>
public class FinMindCrawler : IFinancialCrawler
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FinMindCrawler> _logger;

    private const string FinMindBaseUrl = "https://api.finmindtrade.com/api/v4/data";

    public FinMindCrawler(HttpClient httpClient, ILogger<FinMindCrawler> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<FinancialStatement>> FetchFinancialsAsync(string symbol)
    {
        try
        {
            _logger.LogInformation($"Fetching comprehensive financials for {symbol}");

            var endDate = DateTime.Now.ToString("yyyy-MM-dd");
            var startDate = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd");

            // Fetch all datasets in parallel
            var incomeTask = FetchDataset("TaiwanStockFinancialStatements", symbol, startDate, endDate);
            var balanceTask = FetchDataset("TaiwanStockBalanceSheet", symbol, startDate, endDate);
            var cashFlowTask = FetchDataset("TaiwanStockCashFlowsStatement", symbol, startDate, endDate);
            var dividendTask = FetchDataset("TaiwanStockDividend", symbol, startDate, endDate);

            await Task.WhenAll(incomeTask, balanceTask, cashFlowTask, dividendTask);

            // Group by year-quarter
            var groupedData = new Dictionary<string, Dictionary<string, decimal>>();

            ProcessDataset(await incomeTask, groupedData);
            ProcessDataset(await balanceTask, groupedData);
            ProcessDataset(await cashFlowTask, groupedData);
            ProcessDividendDataset(await dividendTask, groupedData);

            // Convert to FinancialStatement entities
            var financials = new List<FinancialStatement>();

            foreach (var (key, data) in groupedData.OrderByDescending(x => x.Key))
            {
                var parts = key.Split('-');
                var year = int.Parse(parts[0]);
                var quarter = parts.Length > 1 ? int.Parse(parts[1]) : 0;

                // Skip if this is a future period with no data
                if (year > DateTime.Now.Year || (year == DateTime.Now.Year && quarter > ((DateTime.Now.Month - 1) / 3 + 1)))
                {
                    continue;
                }

                // Skip if revenue is 0 (no actual data)
                if (GetValue(data, "Revenue") == 0)
                {
                    continue;
                }

                var statement = new FinancialStatement
                {
                    Year = year,
                    Quarter = quarter,

                    // Income Statement
                    Revenue = GetValue(data, "Revenue"),
                    CostOfRevenue = GetValue(data, "CostOfGoodsSold"),
                    GrossProfit = GetValue(data, "GrossProfit"),
                    OperatingIncome = GetValue(data, "OperatingIncome"),
                    NetIncome = GetValue(data, "IncomeAfterTaxes"),
                    EPS = GetValue(data, "EPS"),
                    EBITDA = GetValue(data, "EBITDA"),

                    // Balance Sheet - Use correct API field names
                    TotalAssets = GetValue(data, "TotalAssets"),
                    TotalLiabilities = GetValue(data, "Liabilities"),
                    TotalEquity = GetValue(data, "Equity"),
                    CashAndCashEquivalents = GetValue(data, "CashAndCashEquivalents"),
                    TotalDebt = GetValue(data, "Liabilities"), // Use total liabilities
                    NetDebt = 0,

                    // Cash Flow
                    OperatingCashFlow = GetValue(data, "CashFlowFromOperatingActivities"),
                    CapitalExpenditure = Math.Abs(GetValue(data, "CapitalExpenditures")),
                    FreeCashFlow = 0,

                    // Dividends - from TaiwanStockDividend dataset
                    Dividends = GetValue(data, "CashEarningsDistribution") + GetValue(data, "StockEarningsDistribution"),

                    // Ratios
                    ROE = 0,
                    ROA = 0,
                    GrossMargin = 0,
                    OperatingMargin = 0,
                    NetMargin = 0,
                    DebtToEquity = 0,
                    CurrentRatio = 0
                };

                // Calculate derived values
                statement.NetDebt = statement.TotalDebt - statement.CashAndCashEquivalents;
                statement.FreeCashFlow = statement.OperatingCashFlow - statement.CapitalExpenditure;

                // Calculate margins
                if (statement.Revenue > 0)
                {
                    statement.GrossMargin = statement.GrossProfit / statement.Revenue;
                    statement.OperatingMargin = statement.OperatingIncome / statement.Revenue;
                    statement.NetMargin = statement.NetIncome / statement.Revenue;
                }

                // Calculate ROE
                if (statement.TotalEquity > 0 && statement.NetIncome > 0)
                {
                    statement.ROE = statement.NetIncome / statement.TotalEquity;
                }

                // Calculate ROA
                if (statement.TotalAssets > 0 && statement.NetIncome > 0)
                {
                    statement.ROA = statement.NetIncome / statement.TotalAssets;
                }

                // Calculate Debt to Equity
                if (statement.TotalEquity > 0)
                {
                    statement.DebtToEquity = statement.TotalDebt / statement.TotalEquity;
                }

                // Calculate Current Ratio
                var currentAssets = GetValue(data, "CurrentAssets");
                var currentLiabilities = GetValue(data, "CurrentLiabilities");
                if (currentLiabilities > 0)
                {
                    statement.CurrentRatio = currentAssets / currentLiabilities;
                }

                financials.Add(statement);
            }

            // Take only annual data (quarter=0) for last 10 years, plus latest quarter if available
            var annualData = financials.Where(f => f.Quarter == 0)
                .OrderByDescending(f => f.Year)
                .Take(10)
                .ToList();

            // Add latest quarterly data
            var latestQuarterly = financials
                .Where(f => f.Quarter > 0)
                .OrderByDescending(f => f.Year)
                .ThenByDescending(f => f.Quarter)
                .FirstOrDefault();

            if (latestQuarterly != null)
            {
                annualData.Insert(0, latestQuarterly);
            }

            _logger.LogInformation($"Successfully fetched {annualData.Count} periods for {symbol}");

            return annualData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching financials for {symbol}");
            return new List<FinancialStatement>();
        }
    }

    private async Task<List<Dictionary<string, object>>> FetchDataset(string dataset, string symbol, string startDate, string endDate)
    {
        try
        {
            var url = $"{FinMindBaseUrl}?dataset={dataset}&data_id={symbol}&start_date={startDate}&end_date={endDate}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"FinMind API returned {response.StatusCode} for {dataset}");
                return new List<Dictionary<string, object>>();
            }

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);

            var dataArray = doc.RootElement.GetProperty("data");
            var result = new List<Dictionary<string, object>>();

            foreach (var item in dataArray.EnumerateArray())
            {
                var dict = new Dictionary<string, object>();
                foreach (var prop in item.EnumerateObject())
                {
                    dict[prop.Name] = prop.Value.Clone();
                }
                result.Add(dict);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching {dataset}");
            return new List<Dictionary<string, object>>();
        }
    }

    private void ProcessDataset(List<Dictionary<string, object>> dataset, Dictionary<string, Dictionary<string, decimal>> groupedData)
    {
        foreach (var item in dataset)
        {
            if (!item.ContainsKey("date") || !item.ContainsKey("type") || !item.ContainsKey("value"))
                continue;

            var dateStr = item["date"].ToString() ?? "";
            var type = item["type"].ToString() ?? "";

            // Parse date to determine year and quarter
            if (!DateTime.TryParse(dateStr, out var date))
                continue;

            var year = date.Year;
            var quarter = 0;

            // Determine quarter based on month
            if (date.Month == 3) quarter = 1;
            else if (date.Month == 6) quarter = 2;
            else if (date.Month == 9) quarter = 3;
            else if (date.Month == 12) quarter = 0; // Q4 = annual
            else continue; // Skip non-quarter-end dates

            var key = quarter == 0 ? $"{year}-0" : $"{year}-{quarter}";

            if (!groupedData.ContainsKey(key))
            {
                groupedData[key] = new Dictionary<string, decimal>();
            }

            decimal value = 0;
            if (item["value"] is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Number)
                {
                    value = jsonElement.GetDecimal();
                }
            }

            groupedData[key][type] = value;
        }
    }

    private void ProcessDividendDataset(List<Dictionary<string, object>> dataset, Dictionary<string, Dictionary<string, decimal>> groupedData)
    {
        foreach (var item in dataset)
        {
            if (!item.ContainsKey("date") || !item.ContainsKey("CashEarningsDistribution"))
                continue;

            var dateStr = item["date"].ToString() ?? "";
            if (!DateTime.TryParse(dateStr, out var date))
                continue;

            // Dividends are typically announced for previous year
            var year = date.Year - 1;
            var key = $"{year}-0"; // Annual data

            if (!groupedData.ContainsKey(key))
            {
                groupedData[key] = new Dictionary<string, decimal>();
            }

            // Extract dividend values
            groupedData[key]["CashEarningsDistribution"] = ExtractDecimal(item["CashEarningsDistribution"]);

            if (item.ContainsKey("StockEarningsDistribution"))
            {
                groupedData[key]["StockEarningsDistribution"] = ExtractDecimal(item["StockEarningsDistribution"]);
            }
        }
    }

    private decimal ExtractDecimal(object value)
    {
        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Number)
            {
                return jsonElement.GetDecimal();
            }
        }
        return 0m;
    }

    private decimal GetValue(Dictionary<string, decimal> data, string key)
    {
        return data.ContainsKey(key) ? data[key] : 0m;
    }
}
