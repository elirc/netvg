using Microsoft.Extensions.DependencyInjection;
using PeblClone.Application.Workforce;

namespace PeblClone.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<WorkforceDashboardService>();
        services.AddScoped<IHiringQuoteCalculator, HiringQuoteCalculator>();

        return services;
    }
}
