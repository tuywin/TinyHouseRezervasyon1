using System;
using System.Collections.Generic;

namespace TinyHouseRezervasyon.Models
{
    public class HomeViewModel
    {
        public required List<Ev> Evler { get; set; }
        public required Filtreler Filtreler { get; set; }
    }

    public class Filtreler
    {
        public required string Sehir { get; set; }
        public DateTime? GirisTarihi { get; set; }
        public DateTime? CikisTarihi { get; set; }
        public int? KisiSayisi { get; set; }
    }
} 