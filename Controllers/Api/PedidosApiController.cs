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
    [Route("api/pedidos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PedidosApiController : ControllerBase
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly IPedidoRepositorio _pedidoRepositorio;
        private readonly BancoContext _bancoContext;

        public PedidosApiController(
            IClienteRepositorio clienteRepositorio,
            IPedidoRepositorio pedidoRepositorio,
            BancoContext bancoContext)
        {
            _clienteRepositorio = clienteRepositorio;
            _pedidoRepositorio = pedidoRepositorio;
            _bancoContext = bancoContext;
        }

        // GET api/pedidos  (equivale a Cliente/HistoricoPedidos)
        [HttpGet]
        public IActionResult Listar()
        {
            var pedidos = _clienteRepositorio
                .BuscarPedidosCliente(User.ClienteId())
                .Select(ParaDto)
                .ToList();

            return Ok(pedidos);
        }

        // POST api/pedidos/5/cancelar  (equivale a Pedido/CancelarPedido)
        [HttpPost("{id:int}/cancelar")]
        public IActionResult Cancelar(int id)
        {
            var clienteId = User.ClienteId();

            var pedido = _bancoContext.Pedidos
                .AsNoTracking()
                .FirstOrDefault(p => p.PedidoId == id && p.ClienteId == clienteId);

            if (pedido == null)
                return NotFound(new { mensagem = "Pedido não encontrado." });

            if (!pedido.Cancelado)
                _pedidoRepositorio.CancelarPedido(id);

            return Ok(new { mensagem = "Pedido cancelado." });
        }

        private static PedidoDto ParaDto(PedidoModel p) => new()
        {
            PedidoId = p.PedidoId,
            PacoteId = p.PacoteId,
            Destino = p.Pacote?.Destino ?? "",
            Preco = p.Preco,
            FormaPagamento = p.FormaPagamento.ToString(),
            Parcelas = p.Parcelas,
            Cancelado = p.Cancelado,
            DataEfetivacao = p.DataEfetivacao,
            DataPagamento = p.DataPagamento,
            NomeHotel = p.Pacote?.ReservaHotel?.NomeHotel,
            DataCheckin = p.Pacote?.ReservaHotel?.DataCheckin,
            DataCheckout = p.Pacote?.ReservaHotel?.DataCheckout,
            NomeInterprete = p.Pacote?.Interprete?.NomeInterprete
        };
    }
}