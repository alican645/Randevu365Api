using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Enum;

namespace Randevu365.Infrastructure.BackgroundJobs;

public class AppointmentCompletionJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AppointmentCompletionJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

    public AppointmentCompletionJob(IServiceScopeFactory scopeFactory, ILogger<AppointmentCompletionJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CompletePassedAppointmentsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu tamamlama job'ında hata oluştu.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CompletePassedAppointmentsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        var passedAppointments = await dbContext.Appointments
            .Where(a => a.Status == AppointmentStatus.Confirmed
                        && !a.IsDeleted
                        && (a.AppointmentDate < today
                            || (a.AppointmentDate == today && a.RequestedEndTime < currentTime)))
            .ToListAsync(cancellationToken);

        if (passedAppointments.Count == 0)
            return;

        foreach (var appointment in passedAppointments)
        {
            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Count} randevu otomatik olarak tamamlandı.", passedAppointments.Count);
    }
}
