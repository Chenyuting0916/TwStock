namespace TwStock.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TwStock.Domain.Entities;
using TwStock.Domain.Interfaces;

public class StockRepository : Repository<Stock>, IStockRepository
{
    public StockRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Stock?> GetBySymbolAsync(string symbol)
    {
        return await _context.Stocks
            .FirstOrDefaultAsync(s => s.Symbol == symbol);
    }

    public async Task<Stock?> GetDetailsBySymbolAsync(string symbol)
    {
        return await _context.Stocks
            .Include(s => s.FinancialStatements)
            .Include(s => s.MarketData)
            .FirstOrDefaultAsync(s => s.Symbol == symbol);
    }
}
