namespace TwStock.Domain.Entities;
using TwStock.Domain.Common;

public class FinancialStatement : BaseEntity
{
    public int StockId { get; set; }
    public Stock? Stock { get; set; }

    public int Year { get; set; }
    public int Quarter { get; set; } // 1-4, 0 for Annual

    // Income Statement (損益表)
    public decimal Revenue { get; set; }
    public decimal CostOfRevenue { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal OperatingIncome { get; set; }
    public decimal NetIncome { get; set; }
    public decimal EPS { get; set; }
    public decimal EBITDA { get; set; }

    // Balance Sheet (資產負債表)
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public decimal CashAndCashEquivalents { get; set; }
    public decimal TotalDebt { get; set; }
    public decimal NetDebt { get; set; }

    // Cash Flow Statement (現金流量表)
    public decimal OperatingCashFlow { get; set; }
    public decimal CapitalExpenditure { get; set; }
    public decimal FreeCashFlow { get; set; }

    // Ratios (關鍵比率)
    public decimal ROE { get; set; }
    public decimal ROA { get; set; }
    public decimal GrossMargin { get; set; }
    public decimal OperatingMargin { get; set; }
    public decimal NetMargin { get; set; }
    public decimal DebtToEquity { get; set; }
    public decimal CurrentRatio { get; set; }
    public decimal Dividends { get; set; }
}
