using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;

namespace SinaisPeloMundo.Repositorio
{
    public class PacoteRepositorio : IPacoteRepositorio
    {
        private readonly BancoContext _bancoContext;
        public PacoteRepositorio(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }
        public List<PacoteModel> BuscarUltimosPacotes()
        {
            return _bancoContext.Pacotes
        .Include(p => p.ReservaHotel)
        .OrderByDescending(p => p.PacoteId)
        .Take(4)
        .ToList();
        }
        public List<PacoteModel> BuscarPacotes()
        {
            return _bancoContext.Pacotes
        .Include(p => p.ReservaHotel)
        .OrderByDescending(p => p.PacoteId)
        .ToList();
        }
    }
}