using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TroskoviRada.Models {
    [Table("odsustvo")]
    public class Odsustvo {
        [Key]
        [Column("id_odsustvo")]
        [Display(Name = "ID")]
        public int IdOdsustvo { get; set; }

        [Required]
        [ForeignKey("Zaposlenik")]
        [Column("id_zaposlenik")]
        [Display(Name = "Zaposlenik")]
        public int IdZaposlenik { get; set; }

        [Required]
        [ForeignKey("TipOdsustva")]
        [Column("id_tip_odsustva")]
        [Display(Name = "Tip odsustva")]
        public int IdTipOdsustva { get; set; }

        [Required(ErrorMessage = "Datum od je obavezno polje")]
        [Column("datum_od")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum od")]
        public DateTime DatumOd { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Datum do je obavezno polje")]
        [Column("datum_do")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum do")]
        public DateTime DatumDo { get; set; } = DateTime.Today;

        [Column("razlog")]
        [StringLength(500)]
        [Display(Name = "Razlog")]
        public string? Razlog { get; set; }

        [Column("odobreno")]
        [Display(Name = "Odobreno")]
        public bool Odobreno { get; set; } = false;

        // Navigacijska svojstva
        [ValidateNever]
        public virtual Zaposlenik Zaposlenik { get; set; } = null!;

        [ValidateNever]
        public virtual TipOdsustva TipOdsustva { get; set; } = null!;
    }
}