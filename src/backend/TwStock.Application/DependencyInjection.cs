namespace TwStock.Application;

using Microsoft.Extensions.DependencyInjection;
using TwStock.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStockService, StockService>();
        return services;
    }
}
