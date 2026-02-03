namespace TwStock.Domain.Interfaces;
using TwStock.Domain.Entities;

public interface IStockRepository : IRepository<Stock>
{
    Task<Stock?> GetBySymbolAsync(string symbol);
    Task<Stock?> GetDetailsBySymbolAsync(string symbol); // Includes financials/market data
}
