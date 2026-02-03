namespace TwStock.Application.Services;
using TwStock.Application.DTOs;

public interface IStockService
{
    Task<StockDto?> GetStockSummaryAsync(string symbol);
    Task<List<FinancialStatementDto>> GetFinancialsAsync(string symbol);
    Task<List<StockDto>> ScreenStocksAsync(decimal? minRoe, decimal? maxPe);
}
