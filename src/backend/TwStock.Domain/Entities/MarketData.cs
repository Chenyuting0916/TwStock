namespace TwStock.Domain.Entities;
using TwStock.Domain.Common;

public class MarketData : BaseEntity
{
    public int StockId { get; set; }
    public Stock? Stock { get; set; }

    public DateTime Date { get; set; }

    // OHLCV
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }

    // Valuation Metrics
    public decimal PeRatio { get; set; } // Price to Earning
    public decimal PbRatio { get; set; } // Price to Book
    public decimal DividendYield { get; set; }
}
