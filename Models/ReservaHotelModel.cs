using System.ComponentModel.DataAnnotations;

namespace SinaisPeloMundo.Models
{
    public class ReservaHotelModel : IValidatableObject
    {
        [Key]
        public int ReservaHotelId { get; set; }

        [Required(ErrorMessage = "Informe o nome do hotel.")]
        [MaxLength(60,
            ErrorMessage = "O nome do hotel pode ter no máximo 60 caracteres.")]
        public string NomeHotel { get; set; }

        [Required(ErrorMessage = "Informe o endereço do hotel.")]
        [MaxLength(60,
            ErrorMessage = "O endereço pode ter no máximo 60 caracteres.")]
        public string EnderecoHotel { get; set; }

        [Required(ErrorMessage = "Informe a data de check-in.")]
        public DateTime DataCheckin { get; set; }

        [Required(ErrorMessage = "Informe a data de check-out.")]
        public DateTime DataCheckout { get; set; }

        [Required(ErrorMessage = "Informe o preço da reserva.")]
        [Range(0.01, 999999,
            ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (DataCheckout <= DataCheckin)
            {
                yield return new ValidationResult(
                    "O check-out deve ser depois do check-in.",
                    new[] { nameof(DataCheckout) });
            }
        }

        public PacoteModel? Pacote { get; set; }
    }
}