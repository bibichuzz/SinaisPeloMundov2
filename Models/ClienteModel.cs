using Microsoft.AspNetCore.Identity;
using SinaisPeloMundo.Helper;
using System.ComponentModel.DataAnnotations;

namespace SinaisPeloMundo.Models
{
    public class ClienteModel
    {
        [Key]
        public int ClienteId { get; set; }

        public List<PedidoModel> Pedidos { get; set; } = new List<PedidoModel>();

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [MaxLength(60, ErrorMessage = "Nome deve ter no máximo 60 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Data de nascimento é obrigatória.")]
        public DateTime DtNascimento { get; set; }

        [Required(ErrorMessage = "Telefone é obrigatório.")]
        [MaxLength(11, ErrorMessage = "Telefone deve ter 11 dígitos.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Telefone deve conter apenas números.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "CPF é obrigatório.")]
        [MaxLength(11, ErrorMessage = "CPF deve ter 11 dígitos.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números.")]
        [CpfValido]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "CEP é obrigatório.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP deve conter 8 números.")]
        [MaxLength(8, ErrorMessage = "CEP deve ter no máximo 8 caracteres.")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "Número é obrigatório.")]
        [MaxLength(15, ErrorMessage = "Número deve ter no máximo 5 caracteres.")]
        public string Numero { get; set; }

        [MaxLength(60, ErrorMessage = "Complemento deve ter no máximo 60 caracteres.")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Bairro é obrigatório.")]
        [MaxLength(60, ErrorMessage = "Bairro deve ter no máximo 60 caracteres.")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Cidade é obrigatório.")]
        [MaxLength(60, ErrorMessage = "Cidade deve ter no máximo 60 caracteres.")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "UF é obrigatório.")]
        [MaxLength(2, ErrorMessage = "UF deve ter no máximo 2 caracteres.")]
        public string Uf { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório.")]
        [MaxLength(60, ErrorMessage = "E-mail deve ter no máximo 60 caracteres.")]
        [EmailAddress(ErrorMessage = "Digite um e-mail válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória.")]
        [MaxLength(255, ErrorMessage = "Senha muito grande.")]
        public string Senha { get; set; }

        public bool Admin { get; set; } = false;

        public void SetSenhaHash()
        {
            var passwordHasher = new PasswordHasher<ClienteModel>();

            Senha = passwordHasher.HashPassword(this, Senha);
        }

        public bool SenhaValida(string senhaDigitada)
        {
            var passwordHasher = new PasswordHasher<ClienteModel>();

            var resultado = passwordHasher.VerifyHashedPassword(
                this,
                Senha,
                senhaDigitada
            );

            return resultado == PasswordVerificationResult.Success;
        }
    }
}
