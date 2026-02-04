using TwStock.Domain.Entities;

namespace TwStock.Application.Interfaces;

public interface IStockCrawler
{
    /// <summary>
    /// Fetches the list of all stocks from the exchange (TWSE/TPEX).
    /// </summary>
    Task<List<Stock>> FetchStockListAsync();
}

public interface IFinancialCrawler
{
    /// <summary>
    /// Fetches historical financial statements for a given stock.
    /// </summary>
    Task<List<FinancialStatement>> FetchFinancialsAsync(string symbol);
}
