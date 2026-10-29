using System.ComponentModel.DataAnnotations;

namespace SinaisPeloMundo.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}
