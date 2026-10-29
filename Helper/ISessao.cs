using SinaisPeloMundo.Models;
namespace SinaisPeloMundo.Helper
{
    public interface ISessao
    {
        void CriarSessao(ClienteModel cliente);
        void EncerrarSessao();
        ClienteModel BuscarSessao();
    }
}
