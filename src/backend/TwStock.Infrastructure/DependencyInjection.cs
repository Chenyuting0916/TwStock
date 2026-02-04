namespace TwStock.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwStock.Domain.Interfaces;
using TwStock.Infrastructure.Persistence;
using TwStock.Infrastructure.Persistence.Repositories;
using TwStock.Infrastructure.Crawlers;
using TwStock.Application.Interfaces;
using TwStock.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IStockRepository, StockRepository>();

        // Crawlers - Use FinMind API for accurate Taiwan stock financial data
        services.AddHttpClient<FinMindCrawler>();
        services.AddScoped<IFinancialCrawler, FinMindCrawler>();
        services.AddHttpClient<TwseStockCrawler>();
        services.AddScoped<IStockCrawler, TwseStockCrawler>();

        // Services
        services.AddScoped<DataUpdateService>();
        services.AddHttpClient<TwStock.Infrastructure.Services.FinMindRawDataService>();
        services.AddScoped<TwStock.Infrastructure.Services.FinMindRawDataService>();
        services.AddScoped<TwStock.Infrastructure.Services.RawDataFinancialService>();

        return services;
    }
}
