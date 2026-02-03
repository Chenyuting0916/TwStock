namespace TwStock.Domain.Entities;
using TwStock.Domain.Common;

public class Stock : BaseEntity
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty; // e.g. "TSE", "OTC"

    // Navigation Properties
    public ICollection<FinancialStatement> FinancialStatements { get; set; } = new List<FinancialStatement>();
    public ICollection<MarketData> MarketData { get; set; } = new List<MarketData>();
}
