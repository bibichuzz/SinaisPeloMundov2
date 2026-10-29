using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Helper;

namespace SinaisPeloMundo.Controllers
{
    public class PacoteController : Controller
    {
        private readonly BancoContext _bancoContext;
        private readonly ISessao _sessao;

        public PacoteController(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }

        public IActionResult DetalhesPacote(int id)
        {
            var pacote = _bancoContext.Pacotes
                .Include(p => p.Passagem)
                .Include(p => p.ReservaHotel)
                .Include(p => p.Interprete)
                .FirstOrDefault(p => p.PacoteId == id);

            if (pacote == null)
            {
                return NotFound();
            }

            return View(pacote);
        }
    }
}