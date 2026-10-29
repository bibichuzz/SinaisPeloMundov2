using System.ComponentModel.DataAnnotations;

namespace SinaisPeloMundo.Models
{
    public class PedidoModel
    {
        [Key]
        public int PedidoId { get; set; }

        // FKs
        [Required]
        public int ClienteId { get; set; }
        [Required]
        public int PacoteId { get; set; }

        // Navegação
        public ClienteModel? Cliente { get; set; }
        public PacoteModel? Pacote { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        public FormaPagamento FormaPagamento { get; set; }

        [Required]
        [Range(1, 12)]
        public int Parcelas { get; set; }

        [Required]
        public bool Cancelado { get; set; } = false;

        [Required]
        public DateTime DataEfetivacao { get; set; } = DateTime.UtcNow;

        public DateTime? DataPagamento { get; set; }
    }
}
