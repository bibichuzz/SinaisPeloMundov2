using SinaisPeloMundo.Models;
using SinaisPeloMundo.Dtos;

namespace SinaisPeloMundo.Repositorio
{
    public interface IPacoteRepositorio
    {
        List<PacoteModel> BuscarUltimosPacotes();

        List<PacoteModel> BuscarPacotes();

        List<PacoteModel> BuscarPacotesParaChatbot();

        List<PacoteChatbotDTO> BuscarPacotesParaRecomendacao();
    }
}