using SinaisPeloMundo.Models;

namespace SinaisPeloMundo.Repositorio
{
    public interface IPacoteRepositorio
    {
        public List<PacoteModel> BuscarUltimosPacotes();
        public List<PacoteModel> BuscarPacotes();
    }
}
