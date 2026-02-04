using Microsoft.AspNetCore.Mvc;
using TwStock.Infrastructure.Services;

namespace TwStock.Api.Controllers;

[ApiController]
[Route("api/stock")]
public class StockController : ControllerBase
{
    private readonly RawDataFinancialService _rawDataService;
    private readonly ILogger<StockController> _logger;

    public StockController(
        RawDataFinancialService rawDataService,
        ILogger<StockController> logger)
    {
        _rawDataService = rawDataService;
        _logger = logger;
    }

    [HttpGet("{symbol}/financials")]
    public async Task<IActionResult> GetFinancials(string symbol)
    {
        try
        {
            var financials = await _rawDataService.GetFinancialsAsync(symbol);
            return Ok(financials);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching financials for {symbol}");
            return StatusCode(500, "Internal server error");
        }
    }
}
