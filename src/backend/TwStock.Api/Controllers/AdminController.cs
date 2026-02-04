using Microsoft.AspNetCore.Mvc;
using TwStock.Application.Services;
using TwStock.Infrastructure.Services;

namespace TwStock.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly DataUpdateService _dataUpdateService;
    private readonly FinMindRawDataService _rawDataService;

    public AdminController(
        DataUpdateService dataUpdateService,
        FinMindRawDataService rawDataService)
    {
        _dataUpdateService = dataUpdateService;
        _rawDataService = rawDataService;
    }

    [HttpPost("update-stocks")]
    public async Task<IActionResult> UpdateStockList()
    {
        await _dataUpdateService.UpdateStockListAsync();
        return Ok("Stock list update started.");
    }

    [HttpPost("update-financials/{symbol}")]
    public async Task<IActionResult> UpdateFinancials(string symbol)
    {
        await _dataUpdateService.UpdateFinancialsAsync(symbol);
        return Ok($"Financials update for {symbol} started.");
    }

    [HttpPost("fetch-raw-data/{symbol}")]
    public async Task<IActionResult> FetchRawData(string symbol)
    {
        await _rawDataService.FetchAndStoreRawDataAsync(symbol);
        return Ok($"Raw data fetch for {symbol} completed.");
    }

    [HttpPost("fetch-all-raw-data")]
    public IActionResult FetchAllRawData()
    {
        // This will take a long time! Run in background
        _ = Task.Run(async () => await _rawDataService.FetchAndStoreAllRawDataAsync());
        return Ok("Raw data fetch for all stocks started in background. This will take several hours.");
    }
}
