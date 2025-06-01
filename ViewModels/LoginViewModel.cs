using System.ComponentModel.DataAnnotations;

namespace TinyHouseRezervasyon.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }
} 