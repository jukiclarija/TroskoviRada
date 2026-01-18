using System.ComponentModel.DataAnnotations;

namespace TroskoviRada.Models {
    public class NaknadaViewModel {
        [Required(ErrorMessage = "Odaberite tip naknade")]
        [Display(Name = "Tip naknade")]
        public int TipNaknadeId { get; set; }

        [Required(ErrorMessage = "Unesite datum")]
        [Display(Name = "Datum početka")]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; } = DateTime.Today;
    }
}