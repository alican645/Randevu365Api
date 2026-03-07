namespace Randevu365.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendPasswordResetEmailAsync(string to, string resetToken);
    Task SendAppointmentConfirmationEmailAsync(string to, string businessName, string date, string time);
    Task SendAppointmentCancellationEmailAsync(string to, string businessName, string date, string reason);
}
