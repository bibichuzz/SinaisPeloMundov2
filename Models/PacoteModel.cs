using System.ComponentModel.DataAnnotations;

namespace SinaisPeloMundo.Models
{
    public class PacoteModel
    {
        [Key]
        public int PacoteId { get; set; }

        public List<PedidoModel> Pedidos { get; set; }
            = new List<PedidoModel>();

        [Range(1, int.MaxValue,
            ErrorMessage = "Selecione uma passagem.")]
        public int PassagemId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Selecione uma reserva.")]
        public int ReservaHotelId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Selecione um intérprete.")]
        public int InterpreteId { get; set; }

        public PassagemModel? Passagem { get; set; }

        public ReservaHotelModel? ReservaHotel { get; set; }

        public InterpreteModel? Interprete { get; set; }

        [Required(ErrorMessage = "Informe o preço.")]
        [Range(0.01, 999999,
            ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Informe a URL da imagem.")]
        [Url(ErrorMessage = "Digite uma URL válida.")]
        [MaxLength(300,
            ErrorMessage = "A URL pode ter no máximo 300 caracteres.")]
        public string UrlImagem { get; set; }

        [Required(ErrorMessage = "Informe o destino.")]
        [MaxLength(50,
            ErrorMessage = "O destino pode ter no máximo 50 caracteres.")]
        public string Destino { get; set; }
    }
}
