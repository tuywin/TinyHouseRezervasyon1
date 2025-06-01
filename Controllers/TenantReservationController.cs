using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyHouseRezervasyon.Data;
using TinyHouseRezervasyon.Models;

namespace TinyHouseRezervasyon.Controllers;

[Authorize(Roles = "Kiraci")]
public class TenantReservationController : Controller
{
    private readonly ApplicationDbContext _context;

    public TenantReservationController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var rezervasyonlar = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .Where(r => r.KullaniciId == userId)
            .OrderByDescending(r => r.GirisTarihi)
            .ToListAsync();

        return View(rezervasyonlar);
    }

    public async Task<IActionResult> Detay(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        var rezervasyon = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .FirstOrDefaultAsync(r => r.Id == id && r.KullaniciId == userId);

        if (rezervasyon == null)
        {
            return NotFound();
        }

        return View(rezervasyon);
    }

    [HttpPost]
    public async Task<IActionResult> Iptal(int id)
    {
        var rezervasyon = await _context.Rezervasyonlar.FindAsync(id);
        if (rezervasyon == null)
        {
            return NotFound();
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (rezervasyon.KullaniciId != userId)
        {
            return Forbid();
        }

        rezervasyon.Durum = RezervasyonDurumu.IptalEdildi;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Onayla(int id)
    {
        var rezervasyon = await _context.Rezervasyonlar.FindAsync(id);
        if (rezervasyon == null)
        {
            return NotFound();
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (rezervasyon.KullaniciId != userId)
        {
            return Forbid();
        }

        rezervasyon.Durum = RezervasyonDurumu.Onaylandi;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Reddet(int id)
    {
        var rezervasyon = await _context.Rezervasyonlar.FindAsync(id);
        if (rezervasyon == null)
        {
            return NotFound();
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (rezervasyon.KullaniciId != userId)
        {
            return Forbid();
        }

        rezervasyon.Durum = RezervasyonDurumu.IptalEdildi;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    public async Task<IActionResult> EvSahibiIletisim(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        var rezervasyon = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .ThenInclude(e => e.EvSahibi)
            .FirstOrDefaultAsync(r => r.Id == id && r.KullaniciId == userId);

        if (rezervasyon == null)
        {
            return NotFound();
        }

        return View(rezervasyon);
    }
} 