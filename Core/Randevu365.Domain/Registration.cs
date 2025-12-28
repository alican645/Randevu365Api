using Microsoft.Extensions.DependencyInjection;

namespace Randevu365.Domain;

public static class Registration
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Domain layer typically doesn't have DI registrations
        // But you can add Domain Services here if needed
        // services.AddScoped<IDomainService, DomainService>();

        return services;
    }
}
