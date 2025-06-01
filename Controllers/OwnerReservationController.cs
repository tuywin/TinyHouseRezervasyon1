using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyHouseRezervasyon.Data;
using TinyHouseRezervasyon.Models;

namespace TinyHouseRezervasyon.Controllers;

[Authorize(Roles = "EvSahibi")]
public class OwnerReservationController : Controller
{
    private readonly ApplicationDbContext _context;

    public OwnerReservationController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var rezervasyonlar = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .Include(r => r.Kullanici)
            .Where(r => r.Ev.EvSahibiId == userId)
            .OrderByDescending(r => r.GirisTarihi)
            .ToListAsync();

        return View(rezervasyonlar);
    }

    [HttpPost]
    public async Task<IActionResult> Onayla(int id)
    {
        var rezervasyon = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rezervasyon == null)
        {
            return NotFound();
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (rezervasyon.Ev.EvSahibiId != userId)
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
        var rezervasyon = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rezervasyon == null)
        {
            return NotFound();
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (rezervasyon.Ev.EvSahibiId != userId)
        {
            return Forbid();
        }

        rezervasyon.Durum = RezervasyonDurumu.IptalEdildi;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    public async Task<IActionResult> Detay(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var rezervasyon = await _context.Rezervasyonlar
            .Include(r => r.Ev)
            .Include(r => r.Kullanici)
            .FirstOrDefaultAsync(r => r.Id == id && r.Ev.EvSahibiId == userId);

        if (rezervasyon == null)
        {
            return NotFound();
        }

        return View(rezervasyon);
    }
} 