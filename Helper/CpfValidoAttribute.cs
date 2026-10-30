using System.ComponentModel.DataAnnotations;

namespace SinaisPeloMundo.Helper
{
    public class CpfValidoAttribute : ValidationAttribute
    {
        public CpfValidoAttribute() : base("CPF inválido.") { }

        public override bool IsValid(object? value)
        {
            // Vazio fica por conta do [Required]
            if (value is not string texto || string.IsNullOrWhiteSpace(texto)) return true;
            return CpfValidador.EhValido(texto);
        }
    }
}