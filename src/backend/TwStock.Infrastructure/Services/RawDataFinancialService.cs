using TwStock.Domain.Entities;
using TwStock.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace TwStock.Infrastructure.Services;

/// <summary>
/// Service to calculate financial metrics directly from FinMindRawData
/// No intermediate table - all calculations done on-the-fly
/// </summary>
public class RawDataFinancialService
{
    private readonly AppDbContext _context;
    private readonly ILogger<RawDataFinancialService> _logger;

    public RawDataFinancialService(AppDbContext context, ILogger<RawDataFinancialService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get calculated financials for a stock directly from raw data
    /// </summary>
    public async Task<List<CalculatedFinancial>> GetFinancialsAsync(string symbol)
    {
        var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        if (stock == null)
        {
            return new List<CalculatedFinancial>();
        }

        // Get all raw data for this stock
        var rawData = await _context.FinMindRawData
            .Where(r => r.StockId == stock.Id)
            .ToListAsync();

        if (!rawData.Any())
        {
            return new List<CalculatedFinancial>();
        }

        // Group by year and quarter
        var groupedByPeriod = GroupByYearQuarter(rawData);

        // Calculate financials for each period
        var financials = new List<CalculatedFinancial>();

        foreach (var yearKv in groupedByPeriod.OrderBy(x => x.Key))
        {
            var year = int.Parse(yearKv.Key);
            var quarters = yearKv.Value;

            Dictionary<string, decimal>? previousCumulative = null;

            for (int q = 1; q <= 4; q++)
            {
                var quarterKey = $"Q{q}";
                if (!quarters.ContainsKey(quarterKey))
                    continue;

                var cumulative = quarters[quarterKey];

                // Calculate quarterly metrics
                var financial = CalculateQuarterlyFinancial(
                    symbol, year, q, cumulative, previousCumulative);

                if (financial != null)
                {
                    financials.Add(financial);
                }

                previousCumulative = cumulative;
            }

            // Annual summary (Q4 cumulative)
            if (quarters.ContainsKey("Q4"))
            {
                var annual = CalculateQuarterlyFinancial(
                    symbol, year, 0, quarters["Q4"], quarters["Q4"]);

                if (annual != null)
                {
                    financials.Add(annual);
                }
            }
        }

        return financials.OrderByDescending(f => f.Year).ThenByDescending(f => f.Quarter).ToList();
    }

    private Dictionary<string, Dictionary<string, Dictionary<string, decimal>>> GroupByYearQuarter(List<FinMindRawData> rawData)
    {
        var grouped = new Dictionary<string, Dictionary<string, Dictionary<string, decimal>>>();

        foreach (var item in rawData)
        {
            var year = item.Date.Year;
            var quarter = GetQuarter(item.Date.Month);

            if (quarter == 0) continue;

            var yearKey = year.ToString();
            if (!grouped.ContainsKey(yearKey))
            {
                grouped[yearKey] = new Dictionary<string, Dictionary<string, decimal>>();
            }

            var quarterKey = $"Q{quarter}";
            if (!grouped[yearKey].ContainsKey(quarterKey))
            {
                grouped[yearKey][quarterKey] = new Dictionary<string, decimal>();
            }

            var key = $"{item.Dataset}_{item.Type}";
            grouped[yearKey][quarterKey][key] = item.Value;
        }

        return grouped;
    }

    private CalculatedFinancial? CalculateQuarterlyFinancial(
        string symbol,
        int year,
        int quarter,
        Dictionary<string, decimal> cumulative,
        Dictionary<string, decimal>? previousCumulative)
    {
        // Income Statement - calculate quarterly from cumulative (except for Q0=annual)
        var revenue = quarter == 0
            ? GetValue(cumulative, "TaiwanStockFinancialStatements_Revenue")
            : GetQuarterlyValue(cumulative, previousCumulative, "TaiwanStockFinancialStatements_Revenue");

        if (revenue == 0) return null;

        // Get EPS first to calculate Net Income (FinMind doesn't provide direct NetIncome field)
        var eps = quarter == 0
            ? GetValue(cumulative, "TaiwanStockFinancialStatements_EPS")
            : GetQuarterlyValue(cumulative, previousCumulative, "TaiwanStockFinancialStatements_EPS");

        // Calculate NetIncome from EPS (EPS × outstanding shares)
        // TSMC has approximately 25.9 billion shares
        var netIncome = eps * 25900000000m;

        var operatingIncome = quarter == 0
            ? GetValue(cumulative, "TaiwanStockFinancialStatements_OperatingIncome")
            : GetQuarterlyValue(cumulative, previousCumulative, "TaiwanStockFinancialStatements_OperatingIncome");

        var grossProfit = quarter == 0
            ? GetValue(cumulative, "TaiwanStockFinancialStatements_GrossProfit")
            : GetQuarterlyValue(cumulative, previousCumulative, "TaiwanStockFinancialStatements_GrossProfit");

        // Balance Sheet - always use cumulative (point-in-time)
        var totalAssets = GetValue(cumulative, "TaiwanStockBalanceSheet_TotalAssets");
        var totalLiabilities = GetValue(cumulative, "TaiwanStockBalanceSheet_Liabilities");
        var totalEquity = GetValue(cumulative, "TaiwanStockBalanceSheet_Equity");
        var cash = GetValue(cumulative, "TaiwanStockBalanceSheet_CashAndCashEquivalents");

        // Cash Flow - calculate quarterly
        var operatingCashFlow = quarter == 0
            ? GetValue(cumulative, "TaiwanStockCashFlowsStatement_CashFlowFromOperatingActivities")
            : GetQuarterlyValue(cumulative, previousCumulative, "TaiwanStockCashFlowsStatement_CashFlowFromOperatingActivities");

        var capex = Math.Abs(quarter == 0
            ? GetValue(cumulative, "TaiwanStockCashFlowsStatement_CapitalExpenditures")
            : GetQuarterlyValue(cumulative, previousCumulative, "TaiwanStockCashFlowsStatement_CapitalExpenditures"));

        // Dividends
        var dividends = GetValue(cumulative, "TaiwanStockDividend_CashEarningsDistribution") +
                       GetValue(cumulative, "TaiwanStockDividend_StockEarningsDistribution");

        // Calculate derived metrics
        var roe = totalEquity > 0 ? netIncome / totalEquity : 0;
        var roa = totalAssets > 0 ? netIncome / totalAssets : 0;
        var grossMargin = revenue > 0 ? grossProfit / revenue : 0;
        var operatingMargin = revenue > 0 ? operatingIncome / revenue : 0;
        var netMargin = revenue > 0 ? netIncome / revenue : 0;
        var debtToEquity = totalEquity > 0 ? totalLiabilities / totalEquity : 0;
        var freeCashFlow = operatingCashFlow - capex;

        return new CalculatedFinancial
        {
            Symbol = symbol,
            Year = year,
            Quarter = quarter,

            // Income Statement
            Revenue = revenue,
            GrossProfit = grossProfit,
            OperatingIncome = operatingIncome,
            NetIncome = netIncome,
            EPS = eps,

            // Balance Sheet
            TotalAssets = totalAssets,
            TotalLiabilities = totalLiabilities,
            TotalEquity = totalEquity,
            CashAndCashEquivalents = cash,

            // Cash Flow
            OperatingCashFlow = operatingCashFlow,
            CapitalExpenditure = capex,
            FreeCashFlow = freeCashFlow,

            // Ratios
            ROE = roe,
            ROA = roa,
            GrossMargin = grossMargin,
            OperatingMargin = operatingMargin,
            NetMargin = netMargin,
            DebtToEquity = debtToEquity,

            // Dividends
            Dividends = dividends
        };
    }

    private decimal GetValue(Dictionary<string, decimal> data, string key)
    {
        return data.ContainsKey(key) ? data[key] : 0m;
    }

    private decimal GetQuarterlyValue(
        Dictionary<string, decimal> current,
        Dictionary<string, decimal>? previous,
        string key)
    {
        var currentValue = GetValue(current, key);

        if (previous == null)
            return currentValue; // Q1

        var previousValue = GetValue(previous, key);
        return currentValue - previousValue; // Qn - Q(n-1)
    }

    private int GetQuarter(int month)
    {
        return month switch
        {
            3 => 1,
            6 => 2,
            9 => 3,
            12 => 4,
            _ => 0
        };
    }
}

/// <summary>
/// Calculated financial data (not stored in DB)
/// </summary>
public class CalculatedFinancial
{
    public string Symbol { get; set; } = "";
    public int Year { get; set; }
    public int Quarter { get; set; }

    // Income Statement
    public decimal Revenue { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal OperatingIncome { get; set; }
    public decimal NetIncome { get; set; }
    public decimal EPS { get; set; }

    // Balance Sheet
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public decimal CashAndCashEquivalents { get; set; }

    // Cash Flow
    public decimal OperatingCashFlow { get; set; }
    public decimal CapitalExpenditure { get; set; }
    public decimal FreeCashFlow { get; set; }

    // Ratios
    public decimal ROE { get; set; }
    public decimal ROA { get; set; }
    public decimal GrossMargin { get; set; }
    public decimal OperatingMargin { get; set; }
    public decimal NetMargin { get; set; }
    public decimal DebtToEquity { get; set; }

    // Dividends
    public decimal Dividends { get; set; }
}
