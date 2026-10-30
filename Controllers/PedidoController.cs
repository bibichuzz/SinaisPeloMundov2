using Microsoft.AspNetCore.Mvc;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Repositorio;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using QRCoder;
using Microsoft.AspNetCore.SignalR;

namespace SinaisPeloMundo.Controllers
{
    public class PedidoController : Controller
    {

        private readonly IPedidoRepositorio _pedidoRepositorio;
        private readonly ISessao _sessao;
        private readonly IDataProtector _protector;
        private readonly IHubContext<PagamentoHub> _hub;

        public PedidoController(IPedidoRepositorio pedidoRepositorio, ISessao sessao,
    IDataProtectionProvider provider, IHubContext<PagamentoHub> hub)
        {
            _pedidoRepositorio = pedidoRepositorio;
            _sessao = sessao;
            _protector = provider.CreateProtector("PagamentoPedido");
            _hub = hub;
        }

        [HttpPost]
        public IActionResult ConfirmarPagamento(int PacoteId, FormaPagamento FormaPagamento, int Parcelas)
        {
            var cliente = _sessao.BuscarSessao();

            if (cliente == null)
            {
                return RedirectToAction("Cadastro", "Cliente");
            }

            var pedido = _pedidoRepositorio.CriarPedido(
                cliente.ClienteId,
                PacoteId,
                FormaPagamento,
                Parcelas);

            if (pedido == null)
            {
                return NotFound();
            }

            return RedirectToAction(
                "Pagamento",
                new { pedidoId = pedido.PedidoId });
        }
        public IActionResult Pagamento(int pedidoId)
        {
            var pedido = _pedidoRepositorio.BuscarPorId(pedidoId);

            if (pedido == null)
            {
                return NotFound();
            }

            string token =
                _protector.Protect(
                    pedido.PedidoId.ToString());

            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            string url = $"{baseUrl}/Pedido/ConfirmarPagamentoQr?token={Uri.EscapeDataString(token)}";

            using var qrGenerator = new QRCoder.QRCodeGenerator();

            var qrCodeData =
                qrGenerator.CreateQrCode(
                    url,
                    QRCoder.QRCodeGenerator.ECCLevel.Q);

            var qrCode =
                new QRCoder.PngByteQRCode(qrCodeData);

            byte[] qrCodeBytes =
                qrCode.GetGraphic(20);

            ViewBag.QrCode =
                "data:image/png;base64," +
                Convert.ToBase64String(qrCodeBytes);

            return View(pedido);
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmarPagamentoQr(string token)
        {
            string idDesprotegido = _protector.Unprotect(token);

            int pedidoId = int.Parse(idDesprotegido);

            var pedido = _pedidoRepositorio.BuscarPorId(pedidoId);

            if (pedido == null)
                return NotFound();

            pedido.DataPagamento = DateTime.UtcNow;

            _pedidoRepositorio.Atualizar(pedido);

            await _hub.Clients
                .Group(pedidoId.ToString())
                .SendAsync("PagamentoConfirmado");

            return Content(
                "Pagamento confirmado! Você já pode voltar ao aplicativo.",
                "text/plain; charset=utf-8");
        }
        [HttpGet]
        public IActionResult CompraEfetuada(int pedidoId)
        {
            var pedido = _pedidoRepositorio.BuscarPorId(pedidoId);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmarCompraEfetuada(int pedidoId)
        {
            var pedido = _pedidoRepositorio.BuscarPorId(pedidoId);

            if (pedido == null)
                return NotFound();

            pedido.DataPagamento = DateTime.UtcNow;

            _pedidoRepositorio.Atualizar(pedido);

            await _hub.Clients
                .Group(pedidoId.ToString())
                .SendAsync("PagamentoConfirmado");

            return Ok();
        }
        public IActionResult RevisarCompra(int id)
        {
            // verifica se existe sessão
            var clienteLogado = _sessao.BuscarSessao();

            // se não estiver logado
            if (clienteLogado == null)
            {
                return RedirectToAction("Cadastro", "Cliente");
            }

            var pacote = _pedidoRepositorio.Detalhes(id);

            return View(pacote);
        }

        [HttpPost]
        public IActionResult CancelarPedido(int pedidoId)
        {
            _pedidoRepositorio.CancelarPedido(pedidoId);

            return RedirectToAction("HistoricoPedidos", "Cliente");
        }

        [HttpGet]
        public IActionResult GerarNotaFiscal(int pedidoId)
        {
            var pedido = _pedidoRepositorio.BuscarPedidoCompleto(pedidoId);

            if (pedido == null)
            {
                return NotFound();
            }

            string nota = $@"
===============================
         NOTA FISCAL
===============================

Pedido: {pedido.PedidoId}

Destino:
{pedido.Pacote.Destino}

--------------------------------
PASSAGEM
--------------------------------

Origem:
{pedido.Pacote.Passagem.LocalPartida}

Data/Hora Saída:
{pedido.Pacote.Passagem.HorarioPartida}

Destino:
{pedido.Pacote.Passagem.LocalChegada}

Data/Hora Chegada:
{pedido.Pacote.Passagem.HorarioChegada}

Transporte:
{pedido.Pacote.Passagem.Transporte}

Tipo:
{pedido.Pacote.Passagem.TipoPassagem}

Placa:
{pedido.Pacote.Passagem.PlacaTransporte}

Poltrona:
{pedido.Pacote.Passagem.Poltrona}

--------------------------------
HOTEL
--------------------------------

Hotel:
{pedido.Pacote.ReservaHotel.NomeHotel}

Endereço:
{pedido.Pacote.ReservaHotel.EnderecoHotel}

Check-in:
{pedido.Pacote.ReservaHotel.DataCheckin:dd/MM/yyyy HH:mm}

Check-out:
{pedido.Pacote.ReservaHotel.DataCheckout:dd/MM/yyyy HH:mm}

--------------------------------
INTÉRPRETE
--------------------------------

Nome:
{pedido.Pacote.Interprete.NomeInterprete}

Email:
{pedido.Pacote.Interprete.Email}

Telefone:
{pedido.Pacote.Interprete.Telefone}

--------------------------------
PAGAMENTO
--------------------------------

Valor:
{pedido.Pacote.Preco:C}

Pagamento efetuado:
{(pedido.DataPagamento != null ? "Sim" : "Não")}

Pedido cancelado:
{(pedido.Cancelado ? "Sim" : "Não")}

===============================
";

            byte[] bytes = Encoding.UTF8.GetBytes(nota);

            return File(
                bytes,
                "text/plain",
                $"NotaFiscal_Pedido_{pedido.PedidoId}.txt"
            );
        }

        [HttpGet]
        public JsonResult VerificarPagamento(int pedidoId)
        {
            var pedido = _pedidoRepositorio.BuscarPorId(pedidoId);

            if (pedido == null)
            {
                return Json(new { pago = false });
            }

            bool pago = pedido.DataPagamento != null;

            return Json(new { pago });
        }
    }
}
