using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TinyHouseRezervasyon.Data;
using TinyHouseRezervasyon.Models;
using TinyHouseRezervasyon.ViewModels;
using System.Security.Claims;

namespace TinyHouseRezervasyon.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var viewModel = new AdminDashboardViewModel
            {
                ToplamEvSayisi = await _context.Evler.CountAsync(),
                ToplamRezervasyonSayisi = await _context.Rezervasyonlar.CountAsync(),
                ToplamKullaniciSayisi = await _context.Users.CountAsync(),
                SonRezervasyonlar = await _context.Rezervasyonlar
                    .Include(r => r.Kullanici)
                    .Include(r => r.Ev)
                    .OrderByDescending(r => r.OlusturmaTarihi)
                    .Take(5)
                    .ToListAsync(),
                SonEklenenEvler = await _context.Evler
                    .OrderByDescending(e => e.OlusturmaTarihi)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Admin dashboard yüklenirken hata oluştu");
            return View("Error");
        }
    }

    public async Task<IActionResult> Kullanicilar()
    {
        try
        {
            var kullanicilar = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FullName,
                    Roller = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name)
                        .ToList()
                })
                .ToListAsync();

            return View(kullanicilar);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcılar listelenirken hata oluştu");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> KullaniciRolDegistir(string userId, string roleName, bool isChecked)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            return Json(new { success = false, message = "Rol bulunamadı." });
        }

        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);

        if (isChecked && userRole == null)
        {
            _context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = role.Id
            });
        }
        else if (!isChecked && userRole != null)
        {
            _context.UserRoles.Remove(userRole);
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    public async Task<IActionResult> Evler()
    {
        try
        {
            var evler = await _context.Evler
                .Include(e => e.EvSahibi)
                .ToListAsync();
            return View(evler);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Evler listelenirken hata oluştu");
            return View("Error");
        }
    }

    public async Task<IActionResult> Rezervasyonlar(string? durum, DateTime? baslangicTarihi, DateTime? bitisTarihi)
    {
        var query = _context.Rezervasyonlar
            .Include(r => r.Kullanici)
            .Include(r => r.Ev)
            .AsQueryable();

        if (!string.IsNullOrEmpty(durum))
        {
            if (Enum.TryParse<RezervasyonDurumu>(durum, out var durumEnum))
            {
                query = query.Where(r => r.Durum == durumEnum);
            }
        }

        if (baslangicTarihi.HasValue)
        {
            query = query.Where(r => r.GirisTarihi >= baslangicTarihi.Value);
        }

        if (bitisTarihi.HasValue)
        {
            query = query.Where(r => r.CikisTarihi <= bitisTarihi.Value);
        }

        var rezervasyonlar = await query
            .OrderByDescending(r => r.OlusturmaTarihi)
            .ToListAsync();

        return View(rezervasyonlar);
    }

    [HttpPost]
    public async Task<IActionResult> EvDurumDegistir(int id, bool aktif)
    {
        try
        {
            var ev = await _context.Evler.FindAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            ev.Aktif = aktif;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ev durumu değiştirilirken hata oluştu");
            return Json(new { success = false, message = "İşlem sırasında bir hata oluştu." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RezervasyonDurumDegistir(int id, RezervasyonDurumu durum)
    {
        try
        {
            var rezervasyon = await _context.Rezervasyonlar.FindAsync(id);
            if (rezervasyon == null)
            {
                return NotFound();
            }

            rezervasyon.Durum = durum;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rezervasyon durumu değiştirilirken hata oluştu");
            return Json(new { success = false, message = "İşlem sırasında bir hata oluştu." });
        }
    }
} 