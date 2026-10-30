using System.Security.Claims;

namespace SinaisPeloMundo.Helper
{
    public static class UsuarioExtensions
    {
        public static int ClienteId(this ClaimsPrincipal usuario)
        {
            var valor = usuario.FindFirstValue("clienteId");
            return int.TryParse(valor, out var id) ? id : 0;
        }
    }
}