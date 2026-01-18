using System.ComponentModel.DataAnnotations;

namespace TroskoviRada.Models {
    public class TipNaknade {
        [Key]
        public int IdTipNaknade { get; set; }

        [Required(ErrorMessage = "Naziv je obavezno polje")]
        [StringLength(100)]
        public string Naziv { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Opis { get; set; }

        [Required(ErrorMessage = "Iznos je obavezno polje")]
        [Range(0, 100000, ErrorMessage = "Iznos mora biti između 0 i 100000")]
        public decimal Iznos { get; set; }

        [Required]
        public bool JePostotak { get; set; } = false;
    }
}