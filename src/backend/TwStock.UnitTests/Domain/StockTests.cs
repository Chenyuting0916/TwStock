namespace TwStock.UnitTests.Domain;

using TwStock.Domain.Entities;
using FluentAssertions;
using Xunit;

public class StockTests
{
    [Fact]
    public void Stock_Should_BeInitialized_With_Empty_Collections()
    {
        // Arrange
        var stock = new Stock();

        // Act & Assert
        stock.FinancialStatements.Should().NotBeNull();
        stock.MarketData.Should().NotBeNull();
        stock.FinancialStatements.Should().BeEmpty();
        stock.MarketData.Should().BeEmpty();
    }
}
