using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TinyHouseRezervasyon.Data;
using TinyHouseRezervasyon.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;

namespace TinyHouseRezervasyon.Controllers;

[Authorize]
public class EvController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EvController> _logger;
    private readonly IWebHostEnvironment _environment;

    public EvController(ApplicationDbContext context, ILogger<EvController> logger, IWebHostEnvironment environment)
    {
        _context = context;
        _logger = logger;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var evler = await _context.Evler
            .Where(e => e.EvSahibiId == userId)
            .OrderByDescending(e => e.OlusturmaTarihi)
            .ToListAsync();

        return View(evler);
    }

    public IActionResult Ekle()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(Ev ev, List<IFormFile> fotograflar)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ApiResponse { Success = false, Message = "Lütfen tüm alanları doldurun." });
        }

        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ev.EvSahibiId = userId;
            ev.OlusturmaTarihi = DateTime.Now;
            ev.Aktif = true;

            if (fotograflar != null && fotograflar.Any())
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "evler");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fotoUrl = await SaveEvFotograflari(fotograflar, uploadsFolder);
                ev.ResimUrl = fotoUrl;
            }

            _context.Evler.Add(ev);
            await _context.SaveChangesAsync();

            return Json(new ApiResponse { Success = true, Message = "Ev başarıyla eklendi." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ev eklenirken hata oluştu");
            return Json(new ApiResponse { Success = false, Message = "Ev eklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin." });
        }
    }

    public async Task<IActionResult> Duzenle(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var ev = await _context.Evler.FirstOrDefaultAsync(e => e.Id == id && e.EvSahibiId == userId);

        if (ev == null)
        {
            return NotFound();
        }

        return View(ev);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(Ev ev, List<IFormFile> fotograflar)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ApiResponse { Success = false, Message = "Lütfen tüm alanları doldurun." });
        }

        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var mevcutEv = await _context.Evler.FirstOrDefaultAsync(e => e.Id == ev.Id && e.EvSahibiId == userId);

            if (mevcutEv == null)
            {
                return Json(new ApiResponse { Success = false, Message = "Ev bulunamadı." });
            }

            mevcutEv.Baslik = ev.Baslik;
            mevcutEv.Aciklama = ev.Aciklama;
            mevcutEv.Sehir = ev.Sehir;
            mevcutEv.Adres = ev.Adres;
            mevcutEv.Fiyat = ev.Fiyat;
            mevcutEv.KisiSayisi = ev.KisiSayisi;
            mevcutEv.YatakSayisi = ev.YatakSayisi;
            mevcutEv.BanyoSayisi = ev.BanyoSayisi;
            mevcutEv.Ozellikler = ev.Ozellikler;

            if (fotograflar != null && fotograflar.Any())
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "evler");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Eski fotoğrafları sil
                if (!string.IsNullOrEmpty(mevcutEv.ResimUrl))
                {
                    var oldPhotoPath = Path.Combine(_environment.WebRootPath, mevcutEv.ResimUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                }

                var fotoUrl = await SaveEvFotograflari(fotograflar, uploadsFolder);
                mevcutEv.ResimUrl = fotoUrl;
            }

            await _context.SaveChangesAsync();

            return Json(new ApiResponse { Success = true, Message = "Ev başarıyla güncellendi." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ev güncellenirken hata oluştu");
            return Json(new ApiResponse { Success = false, Message = "Ev güncellenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var ev = await _context.Evler.FirstOrDefaultAsync(e => e.Id == id && e.EvSahibiId == userId);

            if (ev == null)
            {
                return Json(new ApiResponse { Success = false, Message = "Ev bulunamadı." });
            }

            // Evin fotoğraflarını sil
            if (!string.IsNullOrEmpty(ev.ResimUrl))
            {
                var photoPath = Path.Combine(_environment.WebRootPath, ev.ResimUrl.TrimStart('/'));
                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }
            }

            _context.Evler.Remove(ev);
            await _context.SaveChangesAsync();

            return Json(new ApiResponse { Success = true, Message = "Ev başarıyla silindi." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ev silinirken hata oluştu");
            return Json(new ApiResponse { Success = false, Message = "Ev silinirken bir hata oluştu. Lütfen daha sonra tekrar deneyin." });
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Detay(int id)
    {
        var ev = await _context.Evler
            .Include(e => e.EvSahibi)
            .FirstOrDefaultAsync(e => e.Id == id && e.Aktif);

        if (ev == null)
        {
            return NotFound();
        }

        return View(ev);
    }

    private async Task<string> SaveEvFotograflari(List<IFormFile> fotograflar, string uploadsFolder)
    {
        var fotoUrl = string.Empty;
        var ilkFoto = fotograflar.FirstOrDefault();

        if (ilkFoto != null)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{ilkFoto.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ilkFoto.CopyToAsync(stream);
            }

            fotoUrl = $"/uploads/evler/{uniqueFileName}";
        }

        return fotoUrl;
    }
} 