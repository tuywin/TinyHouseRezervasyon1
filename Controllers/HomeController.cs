using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TinyHouseRezervasyon.Models;
using TinyHouseRezervasyon.Services;
using TinyHouseRezervasyon.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TinyHouseRezervasyon.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, IEmailService emailService, ApplicationDbContext context)
    {
        _logger = logger;
        _emailService = emailService;
        _context = context;
    }

    public async Task<IActionResult> Index(string? sehir, DateTime? girisTarihi, DateTime? cikisTarihi, int? kisiSayisi)
    {
        var query = _context.Evler
            .Include(e => e.Fotograflar)
            .AsQueryable();

        if (!string.IsNullOrEmpty(sehir))
        {
            query = query.Where(e => e.Sehir.Contains(sehir));
        }

        if (kisiSayisi.HasValue)
        {
            query = query.Where(e => e.KisiSayisi >= kisiSayisi.Value);
        }

        if (girisTarihi.HasValue && cikisTarihi.HasValue)
        {
            query = query.Where(e => !e.Rezervasyonlar.Any(r =>
                r.Durum != RezervasyonDurumu.IptalEdildi &&
                ((r.GirisTarihi <= girisTarihi.Value && r.CikisTarihi > girisTarihi.Value) ||
                 (r.GirisTarihi < cikisTarihi.Value && r.CikisTarihi >= cikisTarihi.Value) ||
                 (r.GirisTarihi >= girisTarihi.Value && r.CikisTarihi <= cikisTarihi.Value))));
        }

        var evler = await query.ToListAsync();

        var viewModel = new HomeViewModel
        {
            Evler = evler.ToList(),
            Filtreler = new Filtreler
            {
                Sehir = sehir ?? string.Empty,
                GirisTarihi = girisTarihi,
                CikisTarihi = cikisTarihi,
                KisiSayisi = kisiSayisi
            }
        };

        return View(viewModel);
    }

    public IActionResult Rezervasyon()
    {
        return View();
    }

    public IActionResult Evler(string sehir, int? kisiSayisi, DateTime? girisTarihi, DateTime? cikisTarihi)
    {
        var query = _context.Evler.AsQueryable();

        if (!string.IsNullOrEmpty(sehir))
            query = query.Where(e => e.Sehir.Contains(sehir));
        if (kisiSayisi.HasValue)
            query = query.Where(e => e.KisiSayisi >= kisiSayisi.Value);

        var evler = query.ToList();

        var viewModel = new HomeViewModel
        {
            Evler = evler,
            Filtreler = new Filtreler
            {
                Sehir = sehir ?? string.Empty,
                KisiSayisi = kisiSayisi,
                GirisTarihi = girisTarihi,
                CikisTarihi = cikisTarihi
            }
        };

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("_EvListesi", viewModel.Evler);

        return View(viewModel);
    }

    [Authorize]
    public async Task<IActionResult> Rezervasyonlar()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var rezervasyonlar = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .Where(r => r.KullaniciId == userId)
            .OrderByDescending(r => r.OlusturmaTarihi)
            .ToListAsync();

        return View(rezervasyonlar);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RezervasyonIptal(int id)
    {
        var rezervasyon = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rezervasyon == null)
        {
            return Json(new { success = false, message = "Rezervasyon bulunamadı." });
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || rezervasyon.KullaniciId != userId)
        {
            return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
        }

        if (rezervasyon.Durum != RezervasyonDurumu.Beklemede)
        {
            return Json(new { success = false, message = "Sadece bekleyen rezervasyonlar iptal edilebilir." });
        }

        rezervasyon.Durum = RezervasyonDurumu.IptalEdildi;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> RezervasyonDetay(int id)
    {
        var rezervasyon = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .Include(r => r.Kullanici)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rezervasyon == null)
        {
            return Json(new { success = false, message = "Rezervasyon bulunamadı." });
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || rezervasyon.KullaniciId != userId)
        {
            return Json(new { success = false, message = "Bu işlem için yetkiniz yok." });
        }

        var html = $@"
            <div class='table-responsive'>
                <table class='table'>
                    <tr>
                        <th>Ev:</th>
                        <td>{rezervasyon.Ev.Baslik}</td>
                    </tr>
                    <tr>
                        <th>Giriş Tarihi:</th>
                        <td>{rezervasyon.GirisTarihi.ToShortDateString()}</td>
                    </tr>
                    <tr>
                        <th>Çıkış Tarihi:</th>
                        <td>{rezervasyon.CikisTarihi.ToShortDateString()}</td>
                    </tr>
                    <tr>
                        <th>Kişi Sayısı:</th>
                        <td>{rezervasyon.KisiSayisi}</td>
                    </tr>
                    <tr>
                        <th>Toplam Fiyat:</th>
                        <td>{rezervasyon.ToplamFiyat.ToString("C")}</td>
                    </tr>
                    <tr>
                        <th>Durum:</th>
                        <td>{rezervasyon.Durum}</td>
                    </tr>
                    <tr>
                        <th>Notlar:</th>
                        <td>{rezervasyon.Notlar ?? "-"}</td>
                    </tr>
                </table>
            </div>";

        return Json(new { success = true, data = html });
    }

    public IActionResult Hakkimizda()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Iletisim()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Iletisim(string name, string email, string subject, string message)
    {
        try
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                return Json(new ApiResponse 
                { 
                    Success = false, 
                    Message = "Lütfen tüm alanları doldurun." 
                });
            }

            if (!IsValidEmail(email))
            {
                return Json(new ApiResponse 
                { 
                    Success = false, 
                    Message = "Lütfen geçerli bir e-posta adresi girin." 
                });
            }

            var emailBody = $@"
                <h2>Yeni İletişim Formu Mesajı</h2>
                <p><strong>Gönderen:</strong> {name}</p>
                <p><strong>E-posta:</strong> {email}</p>
                <p><strong>Konu:</strong> {subject}</p>
                <p><strong>Mesaj:</strong></p>
                <p>{message}</p>";

            var success = await _emailService.SendEmailAsync(
                "emiryildiz@gmail.com",
                $"İletişim Formu: {subject}",
                emailBody
            );

            if (success)
            {
                return Json(new ApiResponse 
                { 
                    Success = true, 
                    Message = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız." 
                });
            }
            else
            {
                return Json(new ApiResponse 
                { 
                    Success = false, 
                    Message = "Mesaj gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin." 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İletişim formu gönderilirken hata oluştu");
            return Json(new ApiResponse 
            { 
                Success = false, 
                Message = "Mesaj gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin." 
            });
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
