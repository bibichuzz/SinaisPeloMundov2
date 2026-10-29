using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Repositorio;
using System.Diagnostics;

namespace SinaisPeloMundo.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly ISessao _sessao;

        public ClienteController(IClienteRepositorio clienteRepositorio, ISessao sessao) {
            _clienteRepositorio = clienteRepositorio;
            _sessao = sessao;
        }

        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(ClienteModel cliente)
        {
            // validação de email duplicado
            if (_clienteRepositorio.EmailJaExiste(cliente.Email))
            {
                ModelState.AddModelError("Email", "Email já cadastrado");
            }

            // validação de CPF duplicado
            if (_clienteRepositorio.CpfJaExiste(cliente.Cpf))
            {
                ModelState.AddModelError("Cpf", "CPF já cadastrado");
            }

            if (!ModelState.IsValid)
            {
                return View("Cadastro", cliente);
            }

            _clienteRepositorio.AdicionarCliente(cliente);
            return RedirectToAction("Login", "Login");
        }

        public IActionResult DadosPessoais()
        {
            var cliente = _sessao.BuscarSessao();

            if (cliente == null)
            {
                return RedirectToAction("IndexErro", "Home");
            }

            return View(cliente);
        }

        public IActionResult AlterarDados()
        {
            var cliente = _sessao.BuscarSessao();

            if (cliente == null)
            {
                return RedirectToAction("IndexErro", "Home");
            }

            return View(cliente);
        }

        public IActionResult HistoricoPedidos()
        {
            var cliente = _sessao.BuscarSessao();

            if (cliente == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var pedidos = _clienteRepositorio.BuscarPedidosCliente(cliente.ClienteId);

            return View(pedidos);
        }

        [HttpPost]
        public IActionResult AlterarDados(ClienteModel cliente)
        {
            var clienteSessao = _sessao.BuscarSessao();

            if (clienteSessao == null)
            {
                return RedirectToAction("IndexErro", "Home");
            }

            ModelState.Remove("Senha");

            if (!ModelState.IsValid)
            {
                return View("AlterarDados", cliente);
            }

            var resultado = _clienteRepositorio.AtualizarCliente(cliente);
            if (!resultado.Sucesso)
            {
                ModelState.AddModelError(resultado.Campo, resultado.Mensagem);
                return View("AlterarDados", cliente);
            }

            return RedirectToAction("DadosPessoais");
        }

        [HttpPost]
        public IActionResult ExcluirConta()
        {
            var clienteSessao = _sessao.BuscarSessao();

            if (clienteSessao == null)
            {
                return RedirectToAction("IndexErro", "Home");
            }

            _clienteRepositorio.ExcluirCliente(clienteSessao);
            _sessao.EncerrarSessao();

            return RedirectToAction("Cadastro", "Cliente");
        }        

    }
}