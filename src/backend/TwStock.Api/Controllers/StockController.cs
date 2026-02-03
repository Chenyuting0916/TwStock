namespace TwStock.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using TwStock.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet("{symbol}")]
    public async Task<IActionResult> GetSummary(string symbol)
    {
        var stock = await _stockService.GetStockSummaryAsync(symbol);
        if (stock == null) return NotFound();
        return Ok(stock);
    }
    
    [HttpGet("{symbol}/financials")]
    public async Task<IActionResult> GetFinancials(string symbol)
    {
        var financials = await _stockService.GetFinancialsAsync(symbol);
        return Ok(financials);
    }
}
