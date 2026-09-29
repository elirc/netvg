using Microsoft.Extensions.DependencyInjection;
using PeblClone.Application.Workforce;
using PeblClone.Infrastructure.Workforce;

namespace PeblClone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IWorkforceReadModel, InMemoryWorkforceReadModel>();

        return services;
    }
}
