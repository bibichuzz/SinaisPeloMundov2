using Microsoft.AspNetCore.Mvc;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Repositorio;
using SinaisPeloMundo.ViewModel;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Helper;
using Microsoft.EntityFrameworkCore;

namespace SinaisPeloMundo.Controllers
{
    public class LoginController : Controller
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly ISessao _sessao;
        public LoginController(IClienteRepositorio clienteRepositorio, ISessao sessao)
        {
            _clienteRepositorio = clienteRepositorio;
            _sessao = sessao;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Sair()
        {
            _sessao.EncerrarSessao();
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public IActionResult Logar(LoginViewModel LoginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", LoginViewModel);
            }

            var cliente = _clienteRepositorio.BuscarLogin(LoginViewModel.Email);

            if (cliente == null)
            {
                ModelState.AddModelError("Email", "E-mail não encontrado! Faça seu cadastro.");
                return View("Login", LoginViewModel);
            }

            if (!cliente.SenhaValida(LoginViewModel.Senha))
            {
                ModelState.AddModelError("Senha", "Senha inválida! Tente novamente.");
                return View("Login", LoginViewModel);
            }

            _sessao.CriarSessao(cliente);
            return RedirectToAction("DadosPessoais", "Cliente");
        }
    }
}
