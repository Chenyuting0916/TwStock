using TwStock.Application.Interfaces;
using TwStock.Domain.Entities;
using TwStock.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace TwStock.Application.Services;

public class DataUpdateService
{
    private readonly IStockRepository _repository; // Ideally use UnitOfWork or specific Repos
    private readonly IStockCrawler _stockCrawler;
    private readonly IFinancialCrawler _financialCrawler;
    private readonly ILogger<DataUpdateService> _logger;

    public DataUpdateService(
        IStockRepository repository,
        IStockCrawler stockCrawler,
        IFinancialCrawler financialCrawler,
        ILogger<DataUpdateService> logger)
    {
        _repository = repository;
        _stockCrawler = stockCrawler;
        _financialCrawler = financialCrawler;
        _logger = logger;
    }

    public async Task UpdateStockListAsync()
    {
        _logger.LogInformation("Starting Stock List Update...");
        var stocks = await _stockCrawler.FetchStockListAsync();

        var addedCount = 0;
        foreach (var stock in stocks)
        {
            var exists = await _repository.GetBySymbolAsync(stock.Symbol);
            if (exists == null)
            {
                await _repository.AddAsync(stock); // This already calls SaveChangesAsync
                addedCount++;
                _logger.LogInformation($"Added new stock: {stock.Symbol} {stock.Name}");
            }
        }

        _logger.LogInformation($"Stock list update completed. Added: {addedCount}, Total processed: {stocks.Count}");
    }

    public async Task UpdateFinancialsAsync(string symbol)
    {
        _logger.LogInformation($"Updating financials for {symbol}...");
        var stock = await _repository.GetBySymbolAsync(symbol);
        if (stock == null)
        {
            _logger.LogWarning($"Stock {symbol} not found in DB.");
            return;
        }

        var financials = await _financialCrawler.FetchFinancialsAsync(symbol);
        if (financials.Any())
        {
            // Clear existing or Upsert?
            // For simplicity: Remove old annual data and insert new.
            // Real world: Upsert.

            // Since we lack navigation collection loading in 'GetBySymbol', we might need explicit load.
            // Or assume 'GetDetailsBySymbol' usage.

            // Simplest: Just append new ones for now, assuming empty DB.
            // Ideally: _finsRepo.DeleteRange(existing); _finsRepo.AddRange(new);

            stock.FinancialStatements = financials; // If tracking enabled
            await _repository.UpdateAsync(stock);
            _logger.LogInformation($"Updated {financials.Count} financial records for {symbol}");
        }
    }
}
