using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Repositorio;

namespace SinaisPeloMundo.Controllers
{
    public class AdminController : Controller
    {

        private readonly IAdminRepositorio _adminRepositorio;
        private readonly ISessao _sessao;

        public AdminController(ISessao sessao, IAdminRepositorio adminRepositorio)
        {
            _sessao = sessao;
            _adminRepositorio = adminRepositorio;
        }

        private bool UsuarioEhAdmin()
        {
            var cliente = _sessao.BuscarSessao();

            return cliente != null && cliente.Admin;
        }

        // Pacotes
        public IActionResult ListaPacotes()
        {
            List<PacoteModel> pacotes = _adminRepositorio.BuscarPacotes();

            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(pacotes);
        }
        public IActionResult CadastroPacotes()
        {
            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Passagens =
                    _adminRepositorio.BuscarPassagensDisponiveis();

            ViewBag.ReservasHotel =
                _adminRepositorio.BuscarReservasHotelDisponiveis();

            ViewBag.Interpretes =
                _adminRepositorio.BuscarInterpretes();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CadastroPacotes(PacoteModel pacote)
        {
            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Passagens =
                    _adminRepositorio.BuscarPassagensDisponiveis();

                ViewBag.ReservasHotel =
                    _adminRepositorio.BuscarReservasHotelDisponiveis();

                ViewBag.Interpretes =
                    _adminRepositorio.BuscarInterpretes();

                return View(pacote);
            }

            _adminRepositorio.AdicionarPacote(pacote);

            return RedirectToAction("ListaPacotes");
        }
        public IActionResult EditarPacote(int id)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            PacoteModel pacote =
                _adminRepositorio.BuscarPacotePorId(id);

            if (pacote == null)
                return NotFound();

            ViewBag.Passagens =
                _adminRepositorio
                .BuscarPassagensDisponiveisEditar(
                    pacote.PassagemId);

            ViewBag.ReservasHotel =
                _adminRepositorio
                .BuscarReservasHotelDisponiveisEditar(
                    pacote.ReservaHotelId);

            ViewBag.Interpretes =
                _adminRepositorio.BuscarInterpretes();

            return View(pacote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPacote(PacoteModel pacote)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Passagens =
                    _adminRepositorio
                    .BuscarPassagensDisponiveisEditar(
                        pacote.PassagemId);

                ViewBag.ReservasHotel =
                    _adminRepositorio
                    .BuscarReservasHotelDisponiveisEditar(
                        pacote.ReservaHotelId);

                ViewBag.Interpretes =
                    _adminRepositorio.BuscarInterpretes();

                return View(pacote);
            }

            _adminRepositorio.AtualizarPacote(pacote);

            return RedirectToAction("ListaPacotes");
        }
        [HttpPost]
        public IActionResult ExcluirPacote(int id)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            _adminRepositorio.ExcluirPacote(id);

            return RedirectToAction("ListaPacotes");
        }

        // Intérpretes
        public IActionResult ListaInterprete()
        {
            List<InterpreteModel> interpretes = _adminRepositorio.BuscarInterpretes();

            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(interpretes);
        }

        // Reserva Hotél
        public IActionResult ListaHoteis()
        {
            List<ReservaHotelModel> hoteis = _adminRepositorio.BuscarReservasHotel();

            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(hoteis);
        }

        // Passagem
        public IActionResult ListaPassagens()
        {
            List<PassagemModel> passagens = _adminRepositorio.BuscarPassagens();

            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(passagens);
        }
        public IActionResult CadastroPassagens()
        {
            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CadastroPassagens(PassagemModel passagem)
        {
            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(passagem);
            }

            _adminRepositorio.AdicionarPassagem(passagem);

            return RedirectToAction("ListaPassagens");
        }

        public IActionResult EditarPassagem(int id)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            PassagemModel passagem =
                _adminRepositorio.BuscarPassagemPorId(id);

            if (passagem == null)
                return NotFound();

            return View(passagem);
        }

        [HttpPost]
        public IActionResult EditarPassagem(PassagemModel passagem)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                return View(passagem);
            }

            _adminRepositorio.AtualizarPassagem(passagem);

            return RedirectToAction("ListaPassagens");
        }

        [HttpPost]
        public IActionResult ExcluirPassagem(int id)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            try
            {
                _adminRepositorio.ExcluirPassagem(id);

                TempData["MensagemSucesso"] =
                    "Passagem excluída com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }

            return RedirectToAction("ListaPassagens");
        }

        // Reserva de hotel

        public IActionResult CadastroHoteis()
        {
            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CadastroHoteis(ReservaHotelModel hotel)
        {
            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(hotel);
            }

            _adminRepositorio.AdicionarHotel(hotel);

            return RedirectToAction("ListaHoteis");
        }

        public IActionResult EditarHotel(int id)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            ReservaHotelModel hotel =
                _adminRepositorio.BuscarHotelPorId(id);

            if (hotel == null)
                return NotFound();

            return View(hotel);
        }

        [HttpPost]
        public IActionResult EditarHotel(ReservaHotelModel hotel)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                return View(hotel);
            }

            _adminRepositorio.AtualizarHotel(hotel);

            return RedirectToAction("ListaHoteis");
        }

        [HttpPost]
        public IActionResult ExcluirHotel(int id)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            try
            {
                _adminRepositorio.ExcluirHotel(id);

                TempData["MensagemSucesso"] =
                    "Reserva excluída com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }

            return RedirectToAction("ListaHoteis");
        }

        // Interprete
        public IActionResult CadastroInterprete()
        {
            if (!UsuarioEhAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CadastroInterprete(InterpreteModel interprete)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(interprete);

            try
            {
                _adminRepositorio.AdicionarInterprete(interprete);

                TempData["MensagemSucesso"] =
                    "Intérprete cadastrado com sucesso.";

                return RedirectToAction("ListaInterprete");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("CPF"))
                {
                    ModelState.AddModelError("Cpf", ex.Message);
                }
                else if (ex.Message.Contains("e-mail"))
                {
                    ModelState.AddModelError("Email", ex.Message);
                }
                else
                {
                    ModelState.AddModelError("", ex.Message);
                }

                return View(interprete);
            }
        }

        public IActionResult EditarInterprete(int id)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            InterpreteModel interprete =
                _adminRepositorio.BuscarInterpretePorId(id);

            if (interprete == null)
                return NotFound();

            return View(interprete);
        }

        [HttpPost]
        public IActionResult EditarInterprete(InterpreteModel interprete)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                return View(interprete);
            }

            _adminRepositorio.AtualizarInterprete(interprete);

            return RedirectToAction("ListaInterprete");
        }

        [HttpPost]
        public IActionResult ExcluirInterprete(int id)
        {
            if (!UsuarioEhAdmin())
                return RedirectToAction("Index", "Home");

            _adminRepositorio.ExcluirInterprete(id);

            return RedirectToAction("ListaInterprete");
        }
    }
}