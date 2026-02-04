namespace TwStock.Domain.Entities;

/// <summary>
/// Stores raw data from FinMind API without any processing
/// This allows us to query the original data and reprocess if needed
/// </summary>
public class FinMindRawData
{
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to Stock
    /// </summary>
    public int StockId { get; set; }
    public Stock Stock { get; set; } = null!;

    /// <summary>
    /// FinMind dataset name, e.g., "TaiwanStockFinancialStatements", "TaiwanStockBalanceSheet"
    /// </summary>
    public string Dataset { get; set; } = string.Empty;

    /// <summary>
    /// Date from FinMind API (e.g., 2024-03-31, 2024-06-30)
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Type/field name from FinMind API (e.g., "Revenue", "EPS", "TotalAssets")
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Numeric value from FinMind API
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Original Chinese name from FinMind API (e.g., "營業收入", "基本每股盈餘")
    /// </summary>
    public string? OriginName { get; set; }

    /// <summary>
    /// When this record was created in our database
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this record was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }
}
