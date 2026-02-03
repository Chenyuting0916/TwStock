namespace TwStock.Application.DTOs;

public record StockDto(
    string Symbol,
    string Name,
    string Industry,
    string Market,
    decimal? Price,
    decimal? PeRatio,
    decimal? DividendYield,
    decimal? PbRatio,
    decimal? ROE,
    decimal? ROA,
    decimal? EPS
);
