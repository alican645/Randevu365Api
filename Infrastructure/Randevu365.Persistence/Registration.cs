using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Randevu365.Application.Interfaces;
using Randevu365.Persistence.Context;
using Randevu365.Persistence.Repositories;

namespace Randevu365.Persistence;

public static class Registration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext Registration
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSql"));
        });

        // Repository Registrations
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

        // UnitOfWork Registration
        services.AddScoped<IUnitOfWork, Randevu365.Persistence.UnitOfWork.UnitOfWork>();

        return services;
    }
}