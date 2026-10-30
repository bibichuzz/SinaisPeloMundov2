using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Dtos;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Repositorio;

namespace SinaisPeloMundo.Controllers.Api
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesApiController : ControllerBase
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly BancoContext _bancoContext;

        public ClientesApiController(IClienteRepositorio clienteRepositorio, BancoContext bancoContext)
        {
            _clienteRepositorio = clienteRepositorio;
            _bancoContext = bancoContext;
        }

        // POST api/clientes  (equivale a Cliente/Criar)
        [HttpPost]
        public IActionResult Cadastrar([FromBody] CadastroRequest dados)
        {
            var email = dados.Email.Trim();

            // BuscarLogin ignora maiúsculas; EmailJaExiste não. Usamos os dois.
            if (_clienteRepositorio.BuscarLogin(email) != null || _clienteRepositorio.EmailJaExiste(email))
                ModelState.AddModelError(nameof(dados.Email), "E-mail já cadastrado.");

            if (_clienteRepositorio.CpfJaExiste(dados.Cpf))
                ModelState.AddModelError(nameof(dados.Cpf), "CPF já cadastrado.");

            if (dados.DtNascimento.Date > DateTime.UtcNow.Date)
                ModelState.AddModelError(nameof(dados.DtNascimento), "Data de nascimento inválida.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var cliente = new ClienteModel
            {
                Nome = dados.Nome.Trim(),
                DtNascimento = dados.DtNascimento,
                Telefone = dados.Telefone,
                Cpf = dados.Cpf,
                Cep = dados.Cep,
                Numero = dados.Numero.Trim(),
                Complemento = string.IsNullOrWhiteSpace(dados.Complemento) ? null : dados.Complemento.Trim(),
                Bairro = dados.Bairro.Trim(),
                Cidade = dados.Cidade.Trim(),
                Uf = dados.Uf.Trim().ToUpper(),
                Email = email,
                Senha = dados.Senha,   // o repositório gera o hash
                Admin = false          // nunca vem do app
            };

            _clienteRepositorio.AdicionarCliente(cliente);

            return StatusCode(201, new { mensagem = "Cadastro realizado com sucesso." });
        }

        // GET api/clientes/me  (equivale a Cliente/DadosPessoais)
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult MeusDados()
        {
            var id = User.ClienteId();

            var cliente = _bancoContext.Clientes
                .AsNoTracking()
                .FirstOrDefault(c => c.ClienteId == id);

            if (cliente == null)
                return Unauthorized();   // conta excluída com token ainda válido

            return Ok(ParaDto(cliente));
        }

        public static ClienteDto ParaDto(ClienteModel c) => new()
        {
            ClienteId = c.ClienteId,
            Nome = c.Nome,
            Email = c.Email,
            Cpf = c.Cpf,
            DtNascimento = c.DtNascimento,
            Telefone = c.Telefone,
            Cep = c.Cep,
            Numero = c.Numero,
            Complemento = c.Complemento,
            Bairro = c.Bairro,
            Cidade = c.Cidade,
            Uf = c.Uf
        };
    }
}