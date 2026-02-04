using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwStock.Infrastructure.Persistence;

namespace TwStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly AppDbContext _context;

    public DebugController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("db-stats")]
    public async Task<IActionResult> GetDbStats()
    {
        var canConnect = await _context.Database.CanConnectAsync();
        var stockCount = canConnect ? await _context.Stocks.CountAsync() : 0;
        var financialCount = canConnect ? await _context.FinancialStatements.CountAsync() : 0;

        return Ok(new
        {
            CanConnect = canConnect,
            StockCount = stockCount,
            FinancialStatementCount = financialCount,
            DatabaseName = _context.Database.GetDbConnection().Database,
            DataSource = _context.Database.GetDbConnection().DataSource
        });
    }
}
