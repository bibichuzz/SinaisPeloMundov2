using SinaisPeloMundo.Models;

namespace SinaisPeloMundo.Repositorio
{
    public interface IPedidoRepositorio
    {
        public void CancelarPedido(int pedidoId);
        PedidoModel BuscarPedidoCompleto(int pedidoId);
        public PacoteModel Detalhes(int id);
        public PedidoModel CriarPedido(
            int clienteId,
            int pacoteId,
            FormaPagamento formaPagamento,
            int parcelas);
        public PedidoModel BuscarPorId(int pedidoId);
        public PedidoModel Atualizar(PedidoModel pedido);
    }
}
