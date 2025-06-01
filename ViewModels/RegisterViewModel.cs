using System.ComponentModel.DataAnnotations;
using TinyHouseRezervasyon.Models;

namespace TinyHouseRezervasyon.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad Soyad zorunludur.")]
    [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(100, ErrorMessage = "Şifre en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hesap türü seçiniz.")]
    public UserType UserType { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    public string? Phone { get; set; }
} 