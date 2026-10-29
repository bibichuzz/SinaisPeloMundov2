using SinaisPeloMundo.Models;

namespace SinaisPeloMundo.Repositorio
{
    public interface IClienteRepositorio
    {
        ClienteModel AdicionarCliente(ClienteModel cliente);
        ClienteModel BuscarLogin(string email);
        (bool Sucesso, string Campo, string Mensagem) AtualizarCliente(ClienteModel cliente);
        void ExcluirCliente(ClienteModel cliente);
        public bool EmailJaExiste(string email);
        public bool CpfJaExiste(string cpf);
        public List<PedidoModel> BuscarPedidosCliente(int clienteId);
    }
}
