using System.Collections.Generic;
using TinyHouseRezervasyon.Models;

namespace TinyHouseRezervasyon.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int ToplamEvSayisi { get; set; }
        public int ToplamRezervasyonSayisi { get; set; }
        public int ToplamKullaniciSayisi { get; set; }
        public List<Rezervasyon> SonRezervasyonlar { get; set; } = new List<Rezervasyon>();
        public List<Ev> SonEklenenEvler { get; set; } = new List<Ev>();
    }
} 