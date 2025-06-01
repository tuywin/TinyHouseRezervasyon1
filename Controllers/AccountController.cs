using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TinyHouseRezervasyon.Models;
using TinyHouseRezervasyon.ViewModels;

namespace TinyHouseRezervasyon.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                KayitTarihi = DateTime.Now,
                Aktif = true,
                UserType = model.UserType
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Kullanıcı tipine göre rol atama
                string roleName = model.UserType.ToString();
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return Json(new { success = true, message = "Kayıt başarılı! Hoş geldiniz." });
            }

            var errors = result.Errors.Select(e => e.Description);
            return Json(new { success = false, message = string.Join(", ", errors) });
        }

        var modelErrors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);
        return Json(new { success = false, message = string.Join(", ", modelErrors) });
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Giriş başarılı! Hoş geldiniz." });
                }
            }

            return Json(new { success = false, message = "Geçersiz e-posta veya şifre." });
        }

        var modelErrors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);
        return Json(new { success = false, message = string.Join(", ", modelErrors) });
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
} 