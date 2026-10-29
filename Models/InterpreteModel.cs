using System.ComponentModel.DataAnnotations;

namespace SinaisPeloMundo.Models
{
    public class InterpreteModel
    {
        [Key]
        public int InterpreteId { get; set; }

        [Required(ErrorMessage = "Informe o nome do intérprete.")]
        [MaxLength(60,
            ErrorMessage = "O nome pode ter no máximo 60 caracteres.")]
        public string NomeInterprete { get; set; }

        [Required(ErrorMessage = "Informe a data de nascimento.")]
        public DateTime DtNascimento { get; set; }

        [Required(ErrorMessage = "Informe o telefone.")]
        [MaxLength(11,
            ErrorMessage = "O telefone deve ter no máximo 11 dígitos.")]
        [RegularExpression(@"^\d{11}$",
            ErrorMessage = "Telefone deve ter 11 dígitos.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Informe o CPF.")]
        [MaxLength(11,
            ErrorMessage = "O CPF deve ter no máximo 11 dígitos.")]
        [RegularExpression(@"^\d{11}$",
            ErrorMessage = "CPF deve ter 11 dígitos.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [MaxLength(60,
            ErrorMessage = "O e-mail pode ter no máximo 60 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe o preço da diária.")]
        [Range(0.01, 999999,
            ErrorMessage = "O preço da diária deve ser maior que zero.")]
        public decimal PrecoDiaria { get; set; }

        public List<PacoteModel> Pacotes { get; set; }
            = new List<PacoteModel>();
    }
}