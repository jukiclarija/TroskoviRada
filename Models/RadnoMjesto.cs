using System.ComponentModel.DataAnnotations;

namespace TroskoviRada.Models {
    public class RadnoMjesto {
        [Key]
        public int IdRadnoMjesto { get; set; }

        [Required(ErrorMessage = "Naziv je obavezno polje")]
        [StringLength(100)]
        public string Naziv { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Opis { get; set; }

        [Required(ErrorMessage = "Satnica je obavezno polje")]
        [Range(0, 10000, ErrorMessage = "Satnica mora biti između 0 i 10000")]
        public decimal Satnica { get; set; }
    }
}