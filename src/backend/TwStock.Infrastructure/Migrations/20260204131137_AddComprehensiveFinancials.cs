using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwStock.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComprehensiveFinancials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CapitalExpenditure",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CashAndCashEquivalents",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostOfRevenue",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentRatio",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebtToEquity",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EBITDA",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FreeCashFlow",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossProfit",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetDebt",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetMargin",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OperatingCashFlow",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OperatingMargin",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAssets",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDebt",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalEquity",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalLiabilities",
                table: "FinancialStatements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapitalExpenditure",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "CashAndCashEquivalents",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "CostOfRevenue",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "CurrentRatio",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "DebtToEquity",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "EBITDA",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "FreeCashFlow",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "GrossProfit",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "NetDebt",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "NetMargin",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "OperatingCashFlow",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "OperatingMargin",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "TotalAssets",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "TotalDebt",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "TotalEquity",
                table: "FinancialStatements");

            migrationBuilder.DropColumn(
                name: "TotalLiabilities",
                table: "FinancialStatements");
        }
    }
}
