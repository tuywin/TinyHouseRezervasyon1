namespace TinyHouseRezervasyon.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
    Task SendReservationConfirmationAsync(string to, string fullName, string evBaslik, DateTime girisTarihi, DateTime cikisTarihi, decimal toplamFiyat);
    Task SendReservationCancellationAsync(string to, string fullName, string evBaslik, DateTime girisTarihi, DateTime cikisTarihi);
} 