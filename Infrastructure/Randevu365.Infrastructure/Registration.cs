using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Randevu365.Application.Interfaces;
using Randevu365.Infrastructure.Services;

namespace Randevu365.Infrastructure;

public static class Registration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT Service
        services.AddScoped<IJwtService, JwtService>();

        // TODO: Add other external services here
        // services.AddScoped<IEmailService, EmailService>();
        // services.AddScoped<ISmsService, SmsService>();
        // services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}
