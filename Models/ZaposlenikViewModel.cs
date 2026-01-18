using System.ComponentModel.DataAnnotations;

namespace TroskoviRada.Models {
    public class ZaposlenikViewModel {
        // Osnovni podaci
        [Required(ErrorMessage = "Ime je obavezno polje")]
        [StringLength(20)]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno polje")]
        [StringLength(30)]
        public string Prezime { get; set; } = string.Empty;

        [StringLength(21)]
        [Display(Name = "Broj računa")]
        public string? BrojRacuna { get; set; }

        [EmailAddress(ErrorMessage = "Unesite validan email")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(30)]
        public string? Telefon { get; set; }

        [StringLength(11)]
        [Display(Name = "OIB")]
        public string? Oib { get; set; }

        // Radno mjesto
        [Required(ErrorMessage = "Odaberite radno mjesto")]
        [Display(Name = "Radno mjesto")]
        public int RadnoMjestoId { get; set; }

        [Required(ErrorMessage = "Unesite datum početka")]
        [Display(Name = "Datum početka")]
        [DataType(DataType.Date)]
        public DateTime DatumPocetka { get; set; } = DateTime.Today;

        // Naknade
        public List<NaknadaViewModel> Naknade { get; set; } = new List<NaknadaViewModel>();
    }
}