using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TroskoviRada.Models {
    [Table("tip_odsustva")]
    public class TipOdsustva {
        [Key]
        [Column("id_tip_odsustva")]
        public int IdTipOdsustva { get; set; }

        [Required(ErrorMessage = "Tip je obavezno polje")]
        [Column("tip")]
        [StringLength(50, ErrorMessage = "Tip ne može biti duži od 50 znakova")]
        [Display(Name = "Naziv tipa odsustva")]
        public string Naziv { get; set; } = string.Empty; // Property u kodu se zove "Naziv"

        [Required(ErrorMessage = "Koeficijent isplate je obavezno polje")]
        [Column("koeficijent_isplate")]
        [Range(0, 1, ErrorMessage = "Koeficijent isplate mora biti između 0 i 1")]
        [Display(Name = "Koeficijent isplate")]
        public decimal KoeficijentIsplate { get; set; } = 0;

        public virtual ICollection<Odsustvo> Odsustva { get; set; } = new List<Odsustvo>();
    }
}