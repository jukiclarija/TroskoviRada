using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TroskoviRada.Models {
    public class Naknada {
        [Key]
        public int IdNaknada { get; set; }

        [Required]
        [ForeignKey("Zaposlenik")]
        public int IdZaposlenik { get; set; }

        [Required]
        [ForeignKey("TipNaknade")]
        public int IdTipNaknade { get; set; }

        [Required(ErrorMessage = "Datum je obavezno polje")]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Iznos je obavezno polje")]
        [Range(0, 1000000, ErrorMessage = "Iznos mora biti pozitivan broj")]
        public decimal Iznos { get; set; }

        [StringLength(500)]
        public string? Napomena { get; set; }

        // Navigacijska svojstva
        public virtual Zaposlenik Zaposlenik { get; set; } = null!;
        public virtual TipNaknade TipNaknade { get; set; } = null!;
    }
}