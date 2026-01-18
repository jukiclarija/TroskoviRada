using System.ComponentModel.DataAnnotations;

namespace TroskoviRada.Models {
    public class Smjena {
        [Key]
        public int IdSmjena { get; set; }

        [Required(ErrorMessage = "Naziv smjene je obavezno polje")]
        [StringLength(50)]
        [Display(Name = "Naziv smjene")]
        public string NazivSmjene { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vrijeme početka je obavezno polje")]
        [Display(Name = "Vrijeme početka")]
        [DataType(DataType.Time)]
        public TimeSpan VrijemePocetka { get; set; }

        [Required(ErrorMessage = "Vrijeme završetka je obavezno polje")]
        [Display(Name = "Vrijeme završetka")]
        [DataType(DataType.Time)]
        public TimeSpan VrijemeZavrsetka { get; set; }

        [Required(ErrorMessage = "Koeficijent plaćanja je obavezno polje")]
        [Display(Name = "Koeficijent plaćanja")]
        [Range(0.1, 10.0, ErrorMessage = "Koeficijent mora biti između 0.1 i 10")]
        public decimal KoeficijentPlacanja { get; set; } = 1.0m;
    }
}