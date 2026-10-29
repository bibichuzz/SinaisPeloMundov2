using SinaisPeloMundo.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace SinaisPeloMundo.Helper
{
    public class Sessao : ISessao
    {
        private readonly IHttpContextAccessor _httpContext;
        public Sessao(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public ClienteModel BuscarSessao()
        {
            string sessaoCliente = _httpContext.HttpContext.Session.GetString("sessaoClienteLogado");
            if (string.IsNullOrEmpty(sessaoCliente)) return null;

            return JsonSerializer.Deserialize<ClienteModel>(sessaoCliente);
        }

        public void CriarSessao(ClienteModel cliente)
        {
            string valorCliente = JsonSerializer.Serialize(cliente);
            _httpContext.HttpContext.Session.SetString("sessaoClienteLogado", valorCliente);
        }

        public void EncerrarSessao()
        {
            _httpContext.HttpContext.Session.Remove("sessaoClienteLogado");
        }
    }
}
