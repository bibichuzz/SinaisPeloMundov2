using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Dtos;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Repositorio;
using System.Globalization;
using System.Text;

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
        private readonly IDataProtector _protector;
        private readonly IHubContext<PagamentoHub> _hub;

        public PedidosApiController(
            IClienteRepositorio clienteRepositorio,
            IPedidoRepositorio pedidoRepositorio,
            BancoContext bancoContext,
            IDataProtectionProvider provider,
            IHubContext<PagamentoHub> hub)
        {
            _clienteRepositorio = clienteRepositorio;
            _pedidoRepositorio = pedidoRepositorio;
            _bancoContext = bancoContext;
            _protector = provider.CreateProtector("PagamentoPedido"); // o mesmo do site
            _hub = hub;
        }

        // GET api/pedidos  (histórico)
        [HttpGet]
        public IActionResult Listar()
        {
            var pedidos = _clienteRepositorio
                .BuscarPedidosCliente(User.ClienteId())
                .Select(ParaDto)
                .ToList();

            return Ok(pedidos);
        }

        // GET api/pedidos/5
        [HttpGet("{id:int}")]
        public IActionResult Detalhes(int id)
        {
            var pedido = BuscarDoCliente(id);
            if (pedido == null)
                return NotFound(new { mensagem = "Pedido não encontrado." });

            return Ok(ParaDto(pedido));
        }

        // POST api/pedidos  (equivale a Pedido/ConfirmarPagamento)
        [HttpPost]
        public IActionResult Criar([FromBody] CriarPedidoRequest dados)
        {
            var clienteId = User.ClienteId();

            if (!_bancoContext.Clientes.Any(c => c.ClienteId == clienteId))
                return Unauthorized();

            if (!Enum.TryParse<FormaPagamento>(dados.FormaPagamento, true, out var forma)
                || !Enum.IsDefined(forma))
                return BadRequest(new { mensagem = "Forma de pagamento inválida." });

            // Só o crédito aceita parcelas
            var parcelas = forma == FormaPagamento.Credito ? dados.Parcelas : 1;
            if (parcelas < 1 || parcelas > 12)
                return BadRequest(new { mensagem = "Número de parcelas inválido." });

            var pedido = _pedidoRepositorio.CriarPedido(clienteId, dados.PacoteId, forma, parcelas);
            if (pedido == null)
                return NotFound(new { mensagem = "Pacote não encontrado." });

            var completo = _pedidoRepositorio.BuscarPorId(pedido.PedidoId);
            return StatusCode(201, ParaDto(completo!));
        }

        // GET api/pedidos/5/qr  (link que o QR Code vai conter)
        [HttpGet("{id:int}/qr")]
        public IActionResult Qr(int id)
        {
            var pedido = BuscarDoCliente(id);
            if (pedido == null)
                return NotFound(new { mensagem = "Pedido não encontrado." });
            if (pedido.Cancelado)
                return Conflict(new { mensagem = "Este pedido foi cancelado." });

            var token = _protector.Protect(pedido.PedidoId.ToString());
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = $"{baseUrl}/Pedido/ConfirmarPagamentoQr?token={Uri.EscapeDataString(token)}";

            return Ok(new { url });
        }

        // GET api/pedidos/5/pago  (o app consulta a cada poucos segundos)
        [HttpGet("{id:int}/pago")]
        public IActionResult Pago(int id)
        {
            var pedido = BuscarDoCliente(id);
            if (pedido == null)
                return NotFound(new { mensagem = "Pedido não encontrado." });

            return Ok(new { pago = pedido.DataPagamento != null, cancelado = pedido.Cancelado });
        }

        // POST api/pedidos/5/pagar  (botão "Confirmar pagamento")
        [HttpPost("{id:int}/pagar")]
        public async Task<IActionResult> Pagar(int id)
        {
            var pedido = BuscarDoCliente(id);
            if (pedido == null)
                return NotFound(new { mensagem = "Pedido não encontrado." });
            if (pedido.Cancelado)
                return Conflict(new { mensagem = "Este pedido foi cancelado." });

            if (pedido.DataPagamento == null)
            {
                pedido.DataPagamento = DateTime.UtcNow;
                _pedidoRepositorio.Atualizar(pedido);
                await _hub.Clients.Group(id.ToString()).SendAsync("PagamentoConfirmado");
            }

            return Ok(ParaDto(pedido));
        }

        // POST api/pedidos/5/cancelar
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

        // GET api/pedidos/5/nota-fiscal  (equivale a Pedido/GerarNotaFiscal)
        [HttpGet("{id:int}/nota-fiscal")]
        public IActionResult NotaFiscal(int id)
        {
            var clienteId = User.ClienteId();

            var pedido = _pedidoRepositorio.BuscarPedidoCompleto(id);
            if (pedido == null || pedido.ClienteId != clienteId)
                return NotFound(new { mensagem = "Pedido não encontrado." });

            return Ok(new
            {
                nomeArquivo = $"NotaFiscal_Pedido_{pedido.PedidoId}.txt",
                conteudo = MontarNota(pedido)
            });
        }

        // ---------- auxiliares ----------

        private PedidoModel? BuscarDoCliente(int id)
        {
            var pedido = _pedidoRepositorio.BuscarPorId(id);
            return pedido != null && pedido.ClienteId == User.ClienteId() ? pedido : null;
        }

        private static string MontarNota(PedidoModel pedido)
        {
            var pt = new CultureInfo("pt-BR");
            string Dt(DateTime? d) => d?.ToString("dd/MM/yyyy HH:mm", pt) ?? "-";

            var pacote = pedido.Pacote;
            var passagem = pacote?.Passagem;
            var hotel = pacote?.ReservaHotel;
            var interprete = pacote?.Interprete;

            var sb = new StringBuilder();
            sb.AppendLine("===============================");
            sb.AppendLine("         NOTA FISCAL");
            sb.AppendLine("===============================");
            sb.AppendLine();
            sb.AppendLine($"Pedido: {pedido.PedidoId}");
            sb.AppendLine($"Destino: {pacote?.Destino}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine("PASSAGEM");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Origem: {passagem?.LocalPartida}");
            sb.AppendLine($"Saída: {Dt(passagem?.HorarioPartida)}");
            sb.AppendLine($"Destino: {passagem?.LocalChegada}");
            sb.AppendLine($"Chegada: {Dt(passagem?.HorarioChegada)}");
            sb.AppendLine($"Transporte: {passagem?.Transporte}");
            sb.AppendLine($"Tipo: {passagem?.TipoPassagem}");
            sb.AppendLine($"Placa: {passagem?.PlacaTransporte}");
            sb.AppendLine($"Poltrona: {passagem?.Poltrona}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine("HOTEL");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Hotel: {hotel?.NomeHotel}");
            sb.AppendLine($"Endereço: {hotel?.EnderecoHotel}");
            sb.AppendLine($"Check-in: {Dt(hotel?.DataCheckin)}");
            sb.AppendLine($"Check-out: {Dt(hotel?.DataCheckout)}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine("INTÉRPRETE");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Nome: {interprete?.NomeInterprete}");
            sb.AppendLine($"E-mail: {interprete?.Email}");
            sb.AppendLine($"Telefone: {interprete?.Telefone}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine("PAGAMENTO");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Valor: {pedido.Preco.ToString("C", pt)}");
            sb.AppendLine($"Forma: {pedido.FormaPagamento} ({pedido.Parcelas}x)");
            sb.AppendLine($"Pagamento efetuado: {(pedido.DataPagamento != null ? "Sim" : "Não")}");
            sb.AppendLine($"Pedido cancelado: {(pedido.Cancelado ? "Sim" : "Não")}");
            sb.AppendLine();
            sb.AppendLine("===============================");

            return sb.ToString();
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