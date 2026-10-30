using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SinaisPeloMundo.Dtos;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Repositorio;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SinaisPeloMundo.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly IConfiguration _config;

        public AuthApiController(IClienteRepositorio clienteRepositorio, IConfiguration config)
        {
            _clienteRepositorio = clienteRepositorio;
            _config = config;
        }

        // POST api/auth/login  (equivale a Login/Logar)
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest dados)
        {
            var cliente = _clienteRepositorio.BuscarLogin(dados.Email.Trim());

            // Mesma mensagem para e-mail inexistente e senha errada,
            // para não revelar quais e-mails estão cadastrados.
            if (cliente == null || !cliente.SenhaValida(dados.Senha))
                return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

            return Ok(new LoginResponse
            {
                Token = GerarToken(cliente),
                Cliente = ClientesApiController.ParaDto(cliente)
            });
        }

        private string GerarToken(ClienteModel cliente)
        {
            var chave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            // Sem claim de admin: o painel admin existe só no site.
            var claims = new[]
            {
                new Claim("clienteId", cliente.ClienteId.ToString()),
                new Claim("email", cliente.Email)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}