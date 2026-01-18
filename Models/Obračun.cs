using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TroskoviRada.Models {
    public class Obračun {
        [Key]
        public int IdObračun { get; set; }

        [Required]
        [ForeignKey("Zaposlenik")]
        [Display(Name = "Zaposlenik")]
        public int IdZaposlenik { get; set; }

        [Required(ErrorMessage = "Mjesec je obavezno polje")]
        [Range(1, 12, ErrorMessage = "Mjesec mora biti između 1 i 12")]
        [Display(Name = "Mjesec")]
        public int Mjesec { get; set; }

        [Required(ErrorMessage = "Godina je obavezno polje")]
        [Range(2000, 2100, ErrorMessage = "Godina mora biti između 2000 i 2100")]
        [Display(Name = "Godina")]
        public int Godina { get; set; }

        [Required(ErrorMessage = "Osnovica je obavezno polje")]
        [Range(0, 1000000, ErrorMessage = "Osnovica mora biti pozitivan broj")]
        [Display(Name = "Osnovica (odrađeni sati × satnica)")]
        public decimal Osnovica { get; set; }

        [Required(ErrorMessage = "Ukupne naknade je obavezno polje")]
        [Display(Name = "Ukupne naknade")]
        [Range(0, 1000000, ErrorMessage = "Ukupne naknade moraju biti pozitivan broj")]
        public decimal UkupneNaknade { get; set; }

        [Required(ErrorMessage = "Bruto plaća je obavezno polje")]
        [Display(Name = "Bruto plaća")]
        [Range(0, 1000000, ErrorMessage = "Bruto plaća mora biti pozitivan broj")]
        public decimal BrutoPlaca { get; set; }

        [Required(ErrorMessage = "Porez je obavezno polje")]
        [Range(0, 1000000, ErrorMessage = "Porez mora biti pozitivan broj")]
        [Display(Name = "Porez (25%)")]
        public decimal Porez { get; set; }

        [Required(ErrorMessage = "Neto plaća je obavezno polje")]
        [Display(Name = "Neto plaća")]
        [Range(0, 1000000, ErrorMessage = "Neto plaća mora biti pozitivan broj")]
        public decimal NetoPlaca { get; set; }

        [Required(ErrorMessage = "Datum obračuna je obavezno polje")]
        [Display(Name = "Datum obračuna")]
        [DataType(DataType.Date)]
        public DateTime DatumObračuna { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Isplaćeno")]
        public bool Isplaceno { get; set; } = false;

        [StringLength(1000)]
        [Display(Name = "Napomene")]
        public string? Napomene { get; set; }

        // Navigacijsko svojstvo
        public virtual Zaposlenik? Zaposlenik { get; set; }

        // ========== POMOĆNA SVOJSTVA (NotMapped) ==========

        [NotMapped]
        [Display(Name = "Ukupno sati")]
        public decimal UkupnoSati { get; set; }

        [NotMapped]
        [Display(Name = "Iznos odsustva")]
        public decimal IznosOdsustva { get; set; }

        [NotMapped]
        [Display(Name = "Ukupno za isplatu")]
        public decimal IznosUkupno { get; set; }

        [NotMapped]
        [Display(Name = "Iznos osnovice")]
        public decimal IznosOsnovice {
            get => Osnovica;
            set => Osnovica = value;
        }

        // ========== KONSTRUKTOR ==========
        public Obračun() {
            // Postavi default vrijednosti
            DatumObračuna = DateTime.Now;
            Isplaceno = false;
        }

        // ========== POMOĆNE METODE ==========

        /// <summary>
        /// Izračunaj porez od 25% na bruto plaću
        /// </summary>
        public void IzracunajPorez() {
            Porez = BrutoPlaca * 0.25m;
        }

        /// <summary>
        /// Izračunaj neto plaću (bruto - porez)
        /// </summary>
        public void IzracunajNetoPlacu() {
            NetoPlaca = BrutoPlaca - Porez;
        }

        /// <summary>
        /// Izračunaj bruto plaću (osnovica + naknade - odsustva)
        /// </summary>
        /// <param name="iznosOdsustva">Ukupni iznos odbitka za odsustva</param>
        public void IzracunajBrutoPlacu(decimal iznosOdsustva = 0) {
            BrutoPlaca = Osnovica + UkupneNaknade - iznosOdsustva;
        }

        /// <summary>
        /// Kompletno automatsko izračunavanje plaće
        /// </summary>
        public void IzracunajKompletnoPlacu(decimal iznosOdsustva = 0) {
            IzracunajBrutoPlacu(iznosOdsustva);
            IzracunajPorez();
            IzracunajNetoPlacu();
        }

        /// <summary>
        /// Provjeri valjanost obračuna
        /// </summary>
        public bool JeValjan() {
            return BrutoPlaca >= 0 &&
                   NetoPlaca >= 0 &&
                   Porez >= 0 &&
                   NetoPlaca <= BrutoPlaca;
        }

        /// <summary>
        /// Formatiraj prikaz za ispis
        /// </summary>
        public string FormatirajPrikaz() {
            return $"Obračun za {Mjesec}/{Godina}: Bruto: {BrutoPlaca:C2}, Neto: {NetoPlaca:C2}";
        }

        // ========== STATIČKE METODE ==========

        /// <summary>
        /// Izračunaj osnovicu (sati × satnica)
        /// </summary>
        public static decimal IzracunajOsnovicu(decimal ukupnoSati, decimal satnica) {
            return ukupnoSati * satnica;
        }

        /// <summary>
        /// Izračunaj naknade (broj dana × dnevna naknada)
        /// </summary>
        public static decimal IzracunajNaknade(int brojDanaPrisustva, decimal dnevnaNaknada) {
            return brojDanaPrisustva * dnevnaNaknada;
        }

        /// <summary>
        /// Izračunaj iznos odsustva
        /// </summary>
        public static decimal IzracunajIznosOdsustva(int brojDanaOdsustva, decimal dnevniIznos, decimal koeficijentIsplate) {
            return brojDanaOdsustva * dnevniIznos * koeficijentIsplate;
        }
    }
}