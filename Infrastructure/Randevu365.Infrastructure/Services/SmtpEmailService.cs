using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Randevu365.Application.Interfaces;

namespace Randevu365.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var smtpUser = _configuration["Email:SmtpUser"];
        var smtpPass = _configuration["Email:SmtpPass"];
        var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;

        if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
        {
            _logger.LogWarning("Email configuration is not set. Skipping email to {To}", to);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Randevu365", fromEmail));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(smtpUser, smtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("Email sent to {To} with subject: {Subject}", to, subject);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken)
    {
        var subject = "Randevu365 - Sifre Sifirlama";
        var body = $"<h3>Sifre Sifirlama</h3><p>Sifrenizi sifirlamak icin asagidaki kodu kullanin:</p><p><strong>{resetToken}</strong></p><p>Bu kod 15 dakika gecerlidir.</p>";
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendAppointmentConfirmationEmailAsync(string to, string businessName, string date, string time)
    {
        var subject = "Randevu365 - Randevu Onaylandi";
        var body = $"<h3>Randevunuz Onaylandi</h3><p><strong>{businessName}</strong> isletmesindeki {date} tarihli, {time} saatindeki randevunuz onaylandi.</p>";
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendAppointmentCancellationEmailAsync(string to, string businessName, string date, string reason)
    {
        var subject = "Randevu365 - Randevu Iptal Edildi";
        var body = $"<h3>Randevunuz Iptal Edildi</h3><p><strong>{businessName}</strong> isletmesindeki {date} tarihli randevunuz iptal edildi.</p><p>Neden: {reason}</p>";
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendVerificationEmailAsync(string to, string verificationCode)
    {
        var subject = "Randevu365 - E-posta Doğrulama";
        var body = $"<h3>E-posta Doğrulama</h3><p>Kayıt işleminizi tamamlamak için aşağıdaki doğrulama kodunu kullanın:</p><p><strong>{verificationCode}</strong></p><p>Bu kod 5 dakika geçerlidir.</p>";
        await SendEmailAsync(to, subject, body);
    }
}
