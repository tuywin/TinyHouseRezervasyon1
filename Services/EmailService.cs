using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TinyHouseRezervasyon.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var smtpServer = smtpSettings["Server"];
            var smtpPort = int.Parse(smtpSettings["Port"]);
            var smtpUsername = smtpSettings["Username"];
            var smtpPassword = smtpSettings["Password"];
            var fromEmail = smtpSettings["FromEmail"];
            var fromName = smtpSettings["FromName"];

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword)
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message);
            _logger.LogInformation($"Email sent successfully to {to}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email to {to}");
            return false;
        }
    }

    public async Task SendReservationConfirmationAsync(string to, string fullName, string evBaslik, DateTime girisTarihi, DateTime cikisTarihi, decimal toplamFiyat)
    {
        var subject = "Rezervasyon Onayı";
        var body = $@"
            <h2>Rezervasyon Onayı</h2>
            <p>Sayın {fullName},</p>
            <p>{evBaslik} için rezervasyonunuz onaylanmıştır.</p>
            <p><strong>Giriş Tarihi:</strong> {girisTarihi:dd.MM.yyyy}</p>
            <p><strong>Çıkış Tarihi:</strong> {cikisTarihi:dd.MM.yyyy}</p>
            <p><strong>Toplam Tutar:</strong> {toplamFiyat:C}</p>
            <p>İyi tatiller dileriz!</p>";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendReservationCancellationAsync(string to, string fullName, string evBaslik, DateTime girisTarihi, DateTime cikisTarihi)
    {
        var subject = "Rezervasyon İptali";
        var body = $@"
            <h2>Rezervasyon İptali</h2>
            <p>Sayın {fullName},</p>
            <p>{evBaslik} için rezervasyonunuz iptal edilmiştir.</p>
            <p><strong>Giriş Tarihi:</strong> {girisTarihi:dd.MM.yyyy}</p>
            <p><strong>Çıkış Tarihi:</strong> {cikisTarihi:dd.MM.yyyy}</p>
            <p>Başka bir rezervasyon yapmak isterseniz bizi ziyaret edebilirsiniz.</p>";

        await SendEmailAsync(to, subject, body);
    }
} 