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
                Senha = dados.Senha,
                Admin = false
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
                return Unauthorized();

            return Ok(ParaDto(cliente));
        }

        // PUT api/clientes/me  (equivale a Cliente/AlterarDados)
        [HttpPut("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Atualizar([FromBody] AtualizarClienteRequest dados)
        {
            var id = User.ClienteId();

            var cliente = _bancoContext.Clientes.FirstOrDefault(c => c.ClienteId == id);
            if (cliente == null)
                return Unauthorized();

            var email = dados.Email.Trim();

            if (_bancoContext.Clientes.Any(c => c.ClienteId != id && c.Email.ToLower() == email.ToLower()))
                ModelState.AddModelError(nameof(dados.Email), "E-mail já cadastrado.");

            if (_bancoContext.Clientes.Any(c => c.ClienteId != id && c.Cpf == dados.Cpf))
                ModelState.AddModelError(nameof(dados.Cpf), "CPF já cadastrado.");

            if (dados.DtNascimento.Date > DateTime.UtcNow.Date)
                ModelState.AddModelError(nameof(dados.DtNascimento), "Data de nascimento inválida.");

            var trocarSenha = !string.IsNullOrEmpty(dados.NovaSenha);
            if (trocarSenha &&
                (string.IsNullOrEmpty(dados.SenhaAtual) || !cliente.SenhaValida(dados.SenhaAtual)))
            {
                ModelState.AddModelError(nameof(dados.SenhaAtual), "Senha atual incorreta.");
            }

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            cliente.Nome = dados.Nome.Trim();
            cliente.Email = email;
            cliente.Cpf = dados.Cpf;
            cliente.Telefone = dados.Telefone;
            cliente.DtNascimento = DateTime.SpecifyKind(dados.DtNascimento, DateTimeKind.Utc);
            cliente.Cep = dados.Cep;
            cliente.Numero = dados.Numero.Trim();
            cliente.Complemento = string.IsNullOrWhiteSpace(dados.Complemento) ? null : dados.Complemento.Trim();
            cliente.Bairro = dados.Bairro.Trim();
            cliente.Cidade = dados.Cidade.Trim();
            cliente.Uf = dados.Uf.Trim().ToUpper();

            if (trocarSenha)
            {
                cliente.Senha = dados.NovaSenha!;
                cliente.SetSenhaHash();
            }

            _bancoContext.SaveChanges();

            return Ok(ParaDto(cliente));
        }

        // DELETE api/clientes/me  (equivale a Cliente/ExcluirConta)
        [HttpDelete("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Excluir()
        {
            var id = User.ClienteId();

            var cliente = _bancoContext.Clientes.FirstOrDefault(c => c.ClienteId == id);
            if (cliente == null)
                return Unauthorized();

            try
            {
                _clienteRepositorio.ExcluirCliente(cliente);
            }
            catch (Exception)
            {
                return Conflict(new { mensagem = "Não foi possível excluir a conta. Entre em contato com o suporte." });
            }

            return Ok(new { mensagem = "Conta excluída." });
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