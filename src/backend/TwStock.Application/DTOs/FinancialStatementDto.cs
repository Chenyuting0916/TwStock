namespace TwStock.Application.DTOs;

public record FinancialStatementDto(
    int Year,
    int Quarter,
    decimal Revenue,
    decimal NetIncome,
    decimal EPS,
    decimal ROE,
    decimal ROA,
    decimal Dividends
);
