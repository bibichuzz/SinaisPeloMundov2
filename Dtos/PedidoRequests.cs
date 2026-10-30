using System.ComponentModel.DataAnnotations;

namespace SinaisPeloMundo.Dtos
{
    public class CriarPedidoRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Pacote inválido.")]
        public int PacoteId { get; set; }

        [Required(ErrorMessage = "Informe a forma de pagamento.")]
        public string FormaPagamento { get; set; } = "";

        public int Parcelas { get; set; } = 1;
    }
}