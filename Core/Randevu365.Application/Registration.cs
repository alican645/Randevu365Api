using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Randevu365.Application;

public static class Registration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR Registration
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // TODO: Add FluentValidation when needed
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
