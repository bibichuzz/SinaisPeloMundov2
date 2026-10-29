using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;

namespace SinaisPeloMundo.Repositorio
{
    public class PedidoRepositorio : IPedidoRepositorio
    {
        private readonly BancoContext _bancoContext;
        private readonly ISessao _sessao;
        public PedidoRepositorio(BancoContext bancoContext, ISessao sessao)
        {
            _bancoContext = bancoContext;
            _sessao = sessao;
        }
        public void CancelarPedido(int pedidoId)
        {
            var pedido = _bancoContext.Pedidos
                .FirstOrDefault(p => p.PedidoId == pedidoId);

            if (pedido == null)
            {
                return;
            }
            pedido.Cancelado = true;

            _bancoContext.SaveChanges();
        }
        public PedidoModel BuscarPedidoCompleto(int pedidoId)
        {
            return _bancoContext.Pedidos
                .Include(p => p.Pacote)
                    .ThenInclude(p => p.Passagem)

                .Include(p => p.Pacote)
                    .ThenInclude(p => p.ReservaHotel)

                .Include(p => p.Pacote)
                    .ThenInclude(p => p.Interprete)

                .FirstOrDefault(p => p.PedidoId == pedidoId);
        }

        public PacoteModel Detalhes(int id)
        {
            var pacote = _bancoContext.Pacotes
                .Include(p => p.Passagem)
                .Include(p => p.ReservaHotel)
                .Include(p => p.Interprete)
                .FirstOrDefault(p => p.PacoteId == id);

            return pacote;
        }

        public PedidoModel CriarPedido(
            int clienteId,
            int pacoteId,
            FormaPagamento formaPagamento,
            int parcelas)
        {
            var pacote = _bancoContext.Pacotes
                .FirstOrDefault(p => p.PacoteId == pacoteId);

            if (pacote == null)
            {
                return null;
            }

            PedidoModel pedido = new PedidoModel()
            {
                ClienteId = clienteId,
                PacoteId = pacoteId,
                Preco = pacote.Preco,
                FormaPagamento = formaPagamento,
                Parcelas = parcelas,
                Cancelado = false,
                DataEfetivacao = DateTime.UtcNow
            };

            _bancoContext.Pedidos.Add(pedido);

            _bancoContext.SaveChanges();

            return pedido;
        }
        public PedidoModel BuscarPorId(int pedidoId)
        {
            return _bancoContext.Pedidos
                .Include(p => p.Pacote)
                    .ThenInclude(p => p.Passagem)
                .Include(p => p.Pacote)
                    .ThenInclude(p => p.ReservaHotel)
                .Include(p => p.Pacote)
                    .ThenInclude(p => p.Interprete)
                .FirstOrDefault(p => p.PedidoId == pedidoId);
        }
        public PedidoModel Atualizar(PedidoModel pedido)
        {
            pedido.DataEfetivacao = DateTime.SpecifyKind(
                pedido.DataEfetivacao,
                DateTimeKind.Utc
            );

            if (pedido.DataPagamento.HasValue)
            {
                pedido.DataPagamento = DateTime.SpecifyKind(
                    pedido.DataPagamento.Value,
                    DateTimeKind.Utc
                );
            }
            _bancoContext.Pedidos.Update(pedido);

            _bancoContext.SaveChanges();

            return pedido;
        }

    }
}
