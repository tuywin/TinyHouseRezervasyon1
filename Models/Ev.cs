using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinyHouseRezervasyon.Models;

public class Ev
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
    public string Baslik { get; set; } = null!;

    [Required(ErrorMessage = "Şehir alanı zorunludur.")]
    public string Sehir { get; set; } = null!;

    [Required(ErrorMessage = "İlçe alanı zorunludur.")]
    public string Ilce { get; set; } = null!;

    [Required(ErrorMessage = "Adres alanı zorunludur.")]
    public string Adres { get; set; } = null!;

    [Required(ErrorMessage = "Kişi sayısı alanı zorunludur.")]
    [Range(1, 20, ErrorMessage = "Kişi sayısı 1-20 arasında olmalıdır.")]
    public int KisiSayisi { get; set; }

    [Required(ErrorMessage = "Yatak sayısı alanı zorunludur.")]
    [Range(1, 10, ErrorMessage = "Yatak sayısı 1-10 arasında olmalıdır.")]
    public int YatakSayisi { get; set; }

    [Required(ErrorMessage = "Banyo sayısı alanı zorunludur.")]
    [Range(1, 5, ErrorMessage = "Banyo sayısı 1-5 arasında olmalıdır.")]
    public int BanyoSayisi { get; set; }

    [Required(ErrorMessage = "Fiyat alanı zorunludur.")]
    [Range(0, 100000, ErrorMessage = "Fiyat 0-100000 arasında olmalıdır.")]
    public decimal Fiyat { get; set; }

    public string? ResimUrl { get; set; }

    [Required(ErrorMessage = "Ev sahibi alanı zorunludur.")]
    public string? EvSahibiAdi { get; set; }

    [Required(ErrorMessage = "Ev sahibi ID alanı zorunludur.")]
    public string EvSahibiId { get; set; } = null!;

    public string Aciklama { get; set; } = null!;

    public string Ozellikler { get; set; } = null!;

    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

    public bool Aktif { get; set; } = true;

    public List<Rezervasyon> Rezervasyonlar { get; set; } = new();
    public List<EvFotograf> Fotograflar { get; set; } = new();

    [ForeignKey("EvSahibiId")]
    public ApplicationUser EvSahibi { get; set; }
} 