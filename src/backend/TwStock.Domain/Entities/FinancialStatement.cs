namespace TwStock.Domain.Entities;
using TwStock.Domain.Common;

public class FinancialStatement : BaseEntity
{
    public int StockId { get; set; }
    public Stock? Stock { get; set; }

    public int Year { get; set; }
    public int Quarter { get; set; } // 1-4, 0 for Annual

    // Key Financial Metrics
    public decimal Revenue { get; set; }
    public decimal GrossMargin { get; set; }
    public decimal OperatingIncome { get; set; }
    public decimal NetIncome { get; set; }
    public decimal EPS { get; set; }

    // Ratios
    public decimal ROE { get; set; }
    public decimal ROA { get; set; }
    public decimal Dividends { get; set; }
}
