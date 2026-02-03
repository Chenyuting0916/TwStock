namespace TwStock.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using TwStock.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class ScreenerController : ControllerBase
{
    private readonly IStockService _stockService;

    public ScreenerController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<IActionResult> Screen([FromQuery] decimal? minRoe, [FromQuery] decimal? maxPe)
    {
        var results = await _stockService.ScreenStocksAsync(minRoe, maxPe);
        return Ok(results);
    }
}
