using System.ComponentModel.DataAnnotations;
using SinaisPeloMundo.Helper;

namespace SinaisPeloMundo.Dtos
{
    public class AtualizarClienteRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [MaxLength(60, ErrorMessage = "Nome deve ter no máximo 60 caracteres.")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "Data de nascimento é obrigatória.")]
        public DateTime DtNascimento { get; set; }

        [Required(ErrorMessage = "Telefone é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Telefone deve ter 11 números.")]
        public string Telefone { get; set; } = "";

        [Required(ErrorMessage = "CPF é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve ter 11 números.")]
        [CpfValido]
        public string Cpf { get; set; } = "";

        [Required(ErrorMessage = "CEP é obrigatório.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP deve ter 8 números.")]
        public string Cep { get; set; } = "";

        [Required(ErrorMessage = "Número é obrigatório.")]
        [MaxLength(15, ErrorMessage = "Número deve ter no máximo 15 caracteres.")]
        public string Numero { get; set; } = "";

        [MaxLength(60, ErrorMessage = "Complemento deve ter no máximo 60 caracteres.")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Bairro é obrigatório.")]
        [MaxLength(60, ErrorMessage = "Bairro deve ter no máximo 60 caracteres.")]
        public string Bairro { get; set; } = "";

        [Required(ErrorMessage = "Cidade é obrigatória.")]
        [MaxLength(60, ErrorMessage = "Cidade deve ter no máximo 60 caracteres.")]
        public string Cidade { get; set; } = "";

        [Required(ErrorMessage = "UF é obrigatória.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "UF deve ter 2 letras.")]
        public string Uf { get; set; } = "";

        [Required(ErrorMessage = "E-mail é obrigatório.")]
        [MaxLength(60, ErrorMessage = "E-mail deve ter no máximo 60 caracteres.")]
        [EmailAddress(ErrorMessage = "Digite um e-mail válido.")]
        public string Email { get; set; } = "";

        // Só enviadas quando a pessoa quer trocar a senha
        public string? SenhaAtual { get; set; }

        [MaxLength(100, ErrorMessage = "Senha muito grande.")]
        public string? NovaSenha { get; set; }
    }

    public class PedidoDto
    {
        public int PedidoId { get; set; }
        public int PacoteId { get; set; }
        public string Destino { get; set; } = "";
        public decimal Preco { get; set; }
        public string FormaPagamento { get; set; } = "";
        public int Parcelas { get; set; }
        public bool Cancelado { get; set; }
        public DateTime DataEfetivacao { get; set; }
        public DateTime? DataPagamento { get; set; }
        public string? NomeHotel { get; set; }
        public DateTime? DataCheckin { get; set; }
        public DateTime? DataCheckout { get; set; }
        public string? NomeInterprete { get; set; }
    }
}