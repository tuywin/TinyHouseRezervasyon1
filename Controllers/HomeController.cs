using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TinyHouseRezervasyon.Models;

namespace TinyHouseRezervasyon.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Evler()
    {
        return View();
    }

    public IActionResult Rezervasyon()
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
    public IActionResult Iletisim(string name, string email, string subject, string message)
    {
        // TODO: E-posta gönderme işlemi burada yapılacak
        // Şimdilik sadece başarılı mesajı döndürelim
        return Json(new { success = true, message = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız." });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
