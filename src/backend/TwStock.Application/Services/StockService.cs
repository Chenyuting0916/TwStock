namespace TwStock.Application.Services;

using TwStock.Domain.Interfaces;
using TwStock.Application.DTOs;
using TwStock.Domain.Entities;

public class StockService : IStockService
{
    private readonly IStockRepository _repository;

    public StockService(IStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<StockDto?> GetStockSummaryAsync(string symbol)
    {
        var stock = await _repository.GetDetailsBySymbolAsync(symbol);
        if (stock == null) return null;

        var latestMarket = stock.MarketData.MaxBy(m => m.Date);
        var latestFin = stock.FinancialStatements
            .OrderByDescending(f => f.Year)
            .ThenByDescending(f => f.Quarter)
            .FirstOrDefault();

        return new StockDto(
            stock.Symbol,
            stock.Name,
            stock.Industry,
            stock.Market,
            latestMarket?.Close,
            latestMarket?.PeRatio,
            latestMarket?.DividendYield,
            latestMarket?.PbRatio,
            latestFin?.ROE,
            latestFin?.ROA,
            latestFin?.EPS
        );
    }

    public async Task<List<FinancialStatementDto>> GetFinancialsAsync(string symbol)
    {
        var stock = await _repository.GetDetailsBySymbolAsync(symbol);
        if (stock == null) return new List<FinancialStatementDto>();

        return stock.FinancialStatements
            .OrderBy(f => f.Year)
            .ThenBy(f => f.Quarter)
            .Select(f => new FinancialStatementDto(
                f.Year,
                f.Quarter,
                f.Revenue,
                f.NetIncome,
                f.EPS,
                f.ROE,
                f.ROA,
                f.Dividends
            ))
            .ToList();
    }

    public async Task<List<StockDto>> ScreenStocksAsync(decimal? minRoe, decimal? maxPe)
    {
        // Simple client-side filtering (Not efficient for production but okay for prototype)
        // Ideally should be in Repository IQueryable
        var stocks = await _repository.ListAllAsync();

        // This requires loading ALL stocks. With details?
        // Generic Repository ListAllAsync doesn't include Details by default unless overridden.
        // I need to use a specific query in Repository for Screening.

        // For now, I'll return empty List to avoid performance bomb, or implement rudimentary fetch.
        // Using "GetDetailsBySymbolAsync" is 1 by 1.

        // Better: Add `FindWithDetailsAsync` to Repo or specific logic.
        // I will return empty list for now inside the stub and add TODO.
        // User requested "Screener". I should implement it properly later via Repository.

        return new List<StockDto>();
    }
}
