using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TroskoviRada.Models {
    public class Zaposlenje {
        [Key]
        public int IdZaposlenje { get; set; }

        [Required]
        [ForeignKey("Zaposlenik")]
        public int IdZaposlenik { get; set; }

        [Required]
        [ForeignKey("RadnoMjesto")]
        public int IdRadnoMjesto { get; set; }

        [Required(ErrorMessage = "Datum od je obavezno polje")]
        [Display(Name = "Datum početka")]
        [DataType(DataType.Date)]
        public DateTime DatumOd { get; set; } = DateTime.Now;

        [Display(Name = "Datum završetka")]
        [DataType(DataType.Date)]
        public DateTime? DatumDo { get; set; }

        // Navigacijska svojstva - bez nullable
        public virtual Zaposlenik Zaposlenik { get; set; } = null!;
        public virtual RadnoMjesto RadnoMjesto { get; set; } = null!;
    }
}