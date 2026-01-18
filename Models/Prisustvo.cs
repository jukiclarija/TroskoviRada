using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TroskoviRada.Models {
    public class Prisustvo {
        [Key]
        public int IdPrisustvo { get; set; }

        [Required]
        [ForeignKey("Zaposlenik")]
        public int IdZaposlenik { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan VrijemeDolaska { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan VrijemeOdlaska { get; set; }

        // Polje za povezivanje sa smjenom
        [ForeignKey("Smjena")]
        public int? IdSmjena { get; set; } // Nullable jer možda nije obavezno

        [Display(Name = "Broj sati")]
        [Range(0, 24, ErrorMessage = "Broj sati mora biti između 0 i 24")]
        public decimal? BrojSati { get; set; }

        [Display(Name = "Ukupno odradeno (h)")]
        public decimal UkupnoOdradeno { get; set; }

        [StringLength(500)]
        public string? Napomena { get; set; }

        // Navigacijska svojstva
        public virtual Zaposlenik? Zaposlenik { get; set; }

        // Navigacija na smjenu
        public virtual Smjena? Smjena { get; set; }
    }
}