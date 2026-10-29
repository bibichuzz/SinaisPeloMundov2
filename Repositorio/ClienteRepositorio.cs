using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;
using System.Security.Cryptography.X509Certificates;

namespace SinaisPeloMundo.Repositorio
{
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly BancoContext _bancoContext;
        private readonly ISessao _sessao;
        public ClienteRepositorio(BancoContext bancoContext, ISessao sessao)
        {
            _bancoContext = bancoContext;
            _sessao = sessao;
        }
        public ClienteModel AdicionarCliente(ClienteModel cliente)
        {
            cliente.SetSenhaHash();

            cliente.DtNascimento = DateTime.SpecifyKind(
                cliente.DtNascimento,
                DateTimeKind.Utc
            );
            // Inserção no banco de dados
            _bancoContext.Clientes.Add(cliente);
            _bancoContext.SaveChanges();
            return cliente;
        }

        public ClienteModel BuscarLogin(string email)
        {
            return _bancoContext.Clientes.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
        }

        public (bool Sucesso, string Campo, string Mensagem) AtualizarCliente(ClienteModel cliente)
        {
            var emailJaExiste = _bancoContext.Clientes
                .Any(c => c.Email == cliente.Email && c.ClienteId != cliente.ClienteId);

            var cpfJaExiste = _bancoContext.Clientes
                .Any(c => c.Cpf == cliente.Cpf && c.ClienteId != cliente.ClienteId);

            if (emailJaExiste)
            {
                return (false, "Email", "Email já cadastrado!");
            }

            if (cpfJaExiste)
            {
                return (false, "Cpf", "CPF já cadastrado!");
            }

            var clienteDb = _bancoContext.Clientes
                .FirstOrDefault(c => c.ClienteId == cliente.ClienteId);

            if (clienteDb == null) return (false, "", "Cliente não encontrado!");

            clienteDb.Nome = cliente.Nome;
            clienteDb.Email = cliente.Email;
            clienteDb.Cpf = cliente.Cpf;
            clienteDb.Telefone = cliente.Telefone;
            clienteDb.Cep = cliente.Cep;
            clienteDb.Bairro = cliente.Bairro;
            clienteDb.Uf = cliente.Uf;
            clienteDb.Cidade = cliente.Cidade;
            clienteDb.Numero = cliente.Numero;
            clienteDb.Complemento = cliente.Complemento;
            clienteDb.DtNascimento = DateTime.SpecifyKind(
                cliente.DtNascimento,
                DateTimeKind.Utc
            );

            // Só atualiza se tiver valor
            if (!string.IsNullOrEmpty(cliente.Senha))
            {
                clienteDb.Senha = cliente.Senha;
            }

            _bancoContext.SaveChanges();

            // Atualiza a sessão também
            _sessao.CriarSessao(clienteDb);
            return (true,"","");
        }

        public void ExcluirCliente(ClienteModel cliente)
        {
            _bancoContext.Clientes.Remove(cliente);
            _bancoContext.SaveChanges();
        }

        public bool EmailJaExiste(string email)
        {
            return _bancoContext.Clientes.Any(c => c.Email == email);
        }

        public bool CpfJaExiste(string cpf)
        {
            return _bancoContext.Clientes.Any(c => c.Cpf == cpf);
        }

        public List<PedidoModel> BuscarPedidosCliente(int clienteId)
        {
            return _bancoContext.Pedidos
                .Include(p => p.Pacote)
                    .ThenInclude(p => p.ReservaHotel)
                .Include(p => p.Pacote)
                    .ThenInclude(p => p.Interprete)
                .Include(p => p.Pacote)
                    .ThenInclude(p => p.Passagem)
                .Where(p => p.ClienteId == clienteId)
                .OrderBy(p => p.Cancelado)
                .ThenByDescending(p => p.DataEfetivacao)
                .ToList();
        }
    }
}
