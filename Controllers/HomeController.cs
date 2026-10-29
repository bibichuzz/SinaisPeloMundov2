using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Repositorio;
using System.Diagnostics;

namespace SinaisPeloMundo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISessao _sessao;
        private readonly IPacoteRepositorio _pacoteRepositorio;
        public HomeController(ILogger<HomeController> logger, ISessao sessao, IPacoteRepositorio pacoteRepositorio)
        {
            _logger = logger;
            _sessao = sessao;
            _pacoteRepositorio = pacoteRepositorio;
        }

        public IActionResult IndexErro()
        {
            return View();
        }

        public IActionResult Index()
        {
            var cliente = _sessao.BuscarSessao();

            var pacotes = _pacoteRepositorio
                .BuscarUltimosPacotes();

            return View(pacotes);
        }

        public IActionResult Catalogo()
        {
            var cliente = _sessao.BuscarSessao();

            var pacotes = _pacoteRepositorio
                .BuscarPacotes();

            return View(pacotes);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
