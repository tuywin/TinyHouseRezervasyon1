using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinyHouseRezervasyon.Models
{
    public class EvFotograf
    {
        public int Id { get; set; }

        [Required]
        public int EvId { get; set; }

        [Required]
        public string Url { get; set; }

        public string? Aciklama { get; set; }

        public bool AnaFotograf { get; set; }

        public DateTime YuklemeTarihi { get; set; } = DateTime.Now;

        [ForeignKey("EvId")]
        public required Ev Ev { get; set; }
    }
} 