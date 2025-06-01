using System.ComponentModel.DataAnnotations;

namespace TinyHouseRezervasyon.Models
{
    public class EvFotografi
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;

        public int EvId { get; set; }

        public Ev Ev { get; set; } = null!;
    }
} 