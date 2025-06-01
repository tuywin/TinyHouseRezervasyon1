using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyHouseRezervasyon.Data;
using TinyHouseRezervasyon.Models;
using TinyHouseRezervasyon.Services;

namespace TinyHouseRezervasyon.Controllers;

[Authorize(Roles = "Admin")]
public class AdminReservationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<AdminReservationController> _logger;

    public AdminReservationController(
        ApplicationDbContext context,
        IEmailService emailService,
        ILogger<AdminReservationController> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var rezervasyonlar = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .Include(r => r.Kullanici)
            .OrderByDescending(r => r.OlusturmaTarihi)
            .ToListAsync();

        return View(rezervasyonlar);
    }

    public async Task<IActionResult> Details(int id)
    {
        var rezervasyon = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .Include(r => r.Kullanici)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rezervasyon == null)
        {
            return NotFound();
        }

        return View(rezervasyon);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, RezervasyonDurumu yeniDurum)
    {
        var rezervasyon = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .Include(r => r.Kullanici)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rezervasyon == null)
        {
            return NotFound();
        }

        var eskiDurum = rezervasyon.Durum;
        rezervasyon.Durum = yeniDurum;
        rezervasyon.GuncellemeTarihi = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();

            if (rezervasyon.Kullanici != null && rezervasyon.Ev != null)
            {
                if (yeniDurum == RezervasyonDurumu.Onaylandi)
                {
                    await _emailService.SendReservationConfirmationAsync(
                        rezervasyon.Kullanici.Email,
                        rezervasyon.Kullanici.FullName,
                        rezervasyon.Ev.Baslik,
                        rezervasyon.GirisTarihi,
                        rezervasyon.CikisTarihi,
                        rezervasyon.ToplamFiyat);
                }
                else if (yeniDurum == RezervasyonDurumu.IptalEdildi)
                {
                    await _emailService.SendReservationCancellationAsync(
                        rezervasyon.Kullanici.Email,
                        rezervasyon.Kullanici.FullName,
                        rezervasyon.Ev.Baslik,
                        rezervasyon.GirisTarihi,
                        rezervasyon.CikisTarihi);
                }
            }

            TempData["SuccessMessage"] = "Rezervasyon durumu başarıyla güncellendi.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rezervasyon durumu güncellenirken hata oluştu");
            TempData["ErrorMessage"] = "Rezervasyon durumu güncellenirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
} 