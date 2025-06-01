using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinyHouseRezervasyon.Models;

public class Rezervasyon
{
    public int Id { get; set; }

    [Required]
    public int EvId { get; set; }

    public Ev? Ev { get; set; }

    [Required]
    public string KullaniciId { get; set; } = string.Empty;

    public ApplicationUser? Kullanici { get; set; }

    [Required]
    public DateTime GirisTarihi { get; set; }

    [Required]
    public DateTime CikisTarihi { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Kişi sayısı en az 1 olmalıdır.")]
    public int KisiSayisi { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Toplam fiyat 0'dan büyük olmalıdır.")]
    public decimal ToplamFiyat { get; set; }

    public RezervasyonDurumu Durum { get; set; } = RezervasyonDurumu.Beklemede;

    public string? Notlar { get; set; }

    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

    public DateTime? GuncellemeTarihi { get; set; }
} 