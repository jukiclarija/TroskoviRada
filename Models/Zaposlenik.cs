using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TroskoviRada.Models {
    public class Zaposlenik {
        [Key]
        public int IdZaposlenik { get; set; }

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

        // Navigacijska svojstva
        public virtual ICollection<Zaposlenje> Zaposlenja { get; set; } = new List<Zaposlenje>();
        public virtual ICollection<Prisustvo> Prisustva { get; set; } = new List<Prisustvo>();
        public virtual ICollection<Obračun> Obračuni { get; set; } = new List<Obračun>();
        public virtual ICollection<Odsustvo> Odsustva { get; set; } = new List<Odsustvo>();
        public virtual ICollection<Naknada> Naknade { get; set; } = new List<Naknada>(); // DODANO

        // Helper property za trenutno radno mjesto
        [NotMapped]
        public RadnoMjesto? TrenutnoRadnoMjesto {
            get {
                return Zaposlenja
                    .Where(z => z.DatumDo == null || z.DatumDo >= DateTime.Today)
                    .OrderByDescending(z => z.DatumOd)
                    .FirstOrDefault()?
                    .RadnoMjesto;
            }
        }

        // Helper property za prikaz
        [NotMapped]
        public string PunoIme => $"{Ime} {Prezime}";
    }
}