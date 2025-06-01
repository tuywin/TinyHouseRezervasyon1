using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TinyHouseRezervasyon.Models;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
    public string FullName { get; set; } = string.Empty;

    public UserType UserType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime KayitTarihi { get; set; } = DateTime.Now;

    public bool Aktif { get; set; } = true;

    public string? ProfilResmiUrl { get; set; }

    public string? Adres { get; set; }

    public string? Telefon { get; set; }

    public string? Hakkinda { get; set; }

    public string? Roles { get; set; }

    public ICollection<Ev> Evler { get; set; } = new List<Ev>();
    public ICollection<Rezervasyon> Rezervasyonlar { get; set; } = new List<Rezervasyon>();
}

public enum UserType
{
    Admin,      // Yönetici
    EvSahibi,   // Ev Sahibi
    Kiraci      // Kiracı
} 