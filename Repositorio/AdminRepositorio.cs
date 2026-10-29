using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Models;

namespace SinaisPeloMundo.Repositorio
{
    public class AdminRepositorio : IAdminRepositorio
    {
        private readonly BancoContext _bancoContext;

        public AdminRepositorio(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }

        public List<PacoteModel> BuscarPacotes()
        {
            return _bancoContext.Pacotes
                .Include(p => p.Passagem)
                .Include(p => p.ReservaHotel)
                .Include(p => p.Interprete)
                .OrderByDescending(p => p.PacoteId)
                .ToList();
        }

        public List<PassagemModel> BuscarPassagens()
        {
            return _bancoContext.Passagens
                .OrderBy(p => p.PassagemId)
                .ToList();
        }

        public List<ReservaHotelModel> BuscarReservasHotel()
        {
            return _bancoContext.Hoteis
                .OrderBy(r => r.ReservaHotelId)
                .ToList();
        }

        public List<InterpreteModel> BuscarInterpretes()
        {
            return _bancoContext.Interpretes
                .OrderBy(i => i.InterpreteId)
                .ToList();
        }

        // Pacotes

        public PacoteModel AdicionarPacote(PacoteModel pacote)
        {
            _bancoContext.Pacotes.Add(pacote);

            _bancoContext.SaveChanges();

            return pacote;
        }
        public PacoteModel BuscarPacotePorId(int id)
        {
            return _bancoContext.Pacotes
                .FirstOrDefault(p => p.PacoteId == id);
        }
        public PacoteModel AtualizarPacote(PacoteModel pacote)
        {
            PacoteModel pacoteDB =
                BuscarPacotePorId(pacote.PacoteId);

            if (pacoteDB == null)
                throw new Exception("Pacote não encontrado");

            pacoteDB.PassagemId = pacote.PassagemId;
            pacoteDB.ReservaHotelId = pacote.ReservaHotelId;
            pacoteDB.InterpreteId = pacote.InterpreteId;
            pacoteDB.Preco = pacote.Preco;
            pacoteDB.Destino = pacote.Destino;
            pacoteDB.UrlImagem = pacote.UrlImagem;

            _bancoContext.Pacotes.Update(pacoteDB);
            _bancoContext.SaveChanges();

            return pacoteDB;
        }
        public void ExcluirPacote(int id)
        {
            PacoteModel pacoteDB =
                _bancoContext.Pacotes
                .FirstOrDefault(p => p.PacoteId == id);

            if (pacoteDB == null)
                throw new Exception("Pacote não encontrado");

            _bancoContext.Pacotes.Remove(pacoteDB);
            _bancoContext.SaveChanges();
        }

        public List<PassagemModel> BuscarPassagensDisponiveis()
        {
            return _bancoContext.Passagens
                .Where(p => p.Pacote == null)
                .ToList();
        }
        public List<PassagemModel> BuscarPassagensDisponiveisEditar(int passagemId)
        {
            return _bancoContext.Passagens
                .Where(p =>
                    p.Pacote == null ||
                    p.PassagemId == passagemId)
                .ToList();
        }
        public List<ReservaHotelModel> BuscarReservasHotelDisponiveis()
        {
            return _bancoContext.Hoteis
                .Where(r => r.Pacote == null)
                .ToList();
        }
        public List<ReservaHotelModel> BuscarReservasHotelDisponiveisEditar(int reservaHotelId)
        {
            return _bancoContext.Hoteis
                .Where(r =>
                    r.Pacote == null ||
                    r.ReservaHotelId == reservaHotelId)
                .ToList();
        }

        // Passagens

        public PassagemModel AdicionarPassagem(PassagemModel passagem)
        {
            passagem.HorarioPartida = DateTime.SpecifyKind(
                passagem.HorarioPartida,
                DateTimeKind.Utc
            );

            passagem.HorarioChegada = DateTime.SpecifyKind(
                passagem.HorarioChegada,
                DateTimeKind.Utc
            );

            _bancoContext.Passagens.Add(passagem);

            _bancoContext.SaveChanges();

            return passagem;
        }

        public PassagemModel BuscarPassagemPorId(int id)
        {
            return _bancoContext.Passagens
                .FirstOrDefault(p => p.PassagemId == id);
        }

        public PassagemModel AtualizarPassagem(PassagemModel passagem)
        {
            PassagemModel passagemDB =
                BuscarPassagemPorId(passagem.PassagemId);

            if (passagemDB == null)
                throw new Exception("Passagem não encontrada");

            passagemDB.Transporte = passagem.Transporte;
            passagemDB.TipoPassagem = passagem.TipoPassagem;
            passagemDB.Preco = passagem.Preco;
            passagemDB.Poltrona = passagem.Poltrona;
            passagemDB.PlacaTransporte = passagem.PlacaTransporte;
            passagemDB.HorarioPartida = DateTime.SpecifyKind(
                passagem.HorarioPartida,
                DateTimeKind.Utc
            );
            passagemDB.LocalPartida = passagem.LocalPartida;
            passagemDB.HorarioChegada = DateTime.SpecifyKind(
                passagem.HorarioChegada,
                DateTimeKind.Utc
            );
            passagemDB.LocalChegada = passagem.LocalChegada;

            _bancoContext.Passagens.Update(passagemDB);
            _bancoContext.SaveChanges();

            return passagemDB;
        }

        public void ExcluirPassagem(int id)
        {
            PassagemModel passagemDB =
                _bancoContext.Passagens
                .Include(p => p.Pacote)
                .FirstOrDefault(p => p.PassagemId == id);

            if (passagemDB == null)
                throw new Exception("Passagem não encontrada");

            if (passagemDB.Pacote != null)
                throw new Exception(
                    "Esta passagem está vinculada a um pacote.");

            _bancoContext.Passagens.Remove(passagemDB);

            _bancoContext.SaveChanges();
        }
        // Reservas de Hotel

        public ReservaHotelModel AdicionarHotel(ReservaHotelModel hotel)
        {
            hotel.DataCheckin = DateTime.SpecifyKind(
                hotel.DataCheckin,
                DateTimeKind.Utc
            );      

            hotel.DataCheckout = DateTime.SpecifyKind(
                hotel.DataCheckout,
                DateTimeKind.Utc
            );

            _bancoContext.Hoteis.Add(hotel);

            _bancoContext.SaveChanges();

            return hotel;
        }

        public ReservaHotelModel BuscarHotelPorId(int id)
        {
            return _bancoContext.Hoteis
                .FirstOrDefault(h => h.ReservaHotelId == id);
        }

        public ReservaHotelModel AtualizarHotel(ReservaHotelModel hotel)
        {
            ReservaHotelModel hotelDB =
                BuscarHotelPorId(hotel.ReservaHotelId);

            if (hotelDB == null)
                throw new Exception("Reserva de hotel não encontrada");

            hotelDB.NomeHotel = hotel.NomeHotel;
            hotelDB.EnderecoHotel = hotel.EnderecoHotel;
            hotelDB.DataCheckin = DateTime.SpecifyKind(
                hotel.DataCheckin,
                DateTimeKind.Utc
            );

            hotelDB.DataCheckout = DateTime.SpecifyKind(
                hotel.DataCheckout,
                DateTimeKind.Utc
            );
            hotelDB.Preco = hotel.Preco;

            _bancoContext.Hoteis.Update(hotelDB);
            _bancoContext.SaveChanges();

            return hotelDB;
        }

        public void ExcluirHotel(int id)
        {
            ReservaHotelModel hotelDB =
                _bancoContext.Hoteis
                .Include(h => h.Pacote)
                .FirstOrDefault(h => h.ReservaHotelId == id);

            if (hotelDB == null)
                throw new Exception("Reserva não encontrada");

            if (hotelDB.Pacote != null)
                throw new Exception(
                    "Esta reserva está vinculada a um pacote.");

            _bancoContext.Hoteis.Remove(hotelDB);

            _bancoContext.SaveChanges();
        }
        // Intérpretes

        public InterpreteModel AdicionarInterprete(InterpreteModel interprete)
        {
            bool cpfExiste = _bancoContext.Interpretes
                .Any(i => i.Cpf == interprete.Cpf);

            if (cpfExiste)
                throw new Exception("Já existe um intérprete com este CPF.");

            bool emailExiste = _bancoContext.Interpretes
                .Any(i => i.Email == interprete.Email);

            if (emailExiste)
                throw new Exception("Já existe um intérprete com este e-mail.");

            interprete.DtNascimento = DateTime.SpecifyKind(
                interprete.DtNascimento,
                DateTimeKind.Utc
            );

            _bancoContext.Interpretes.Add(interprete);

            _bancoContext.SaveChanges();

            return interprete;
        }

        public InterpreteModel BuscarInterpretePorId(int id)
        {
            return _bancoContext.Interpretes
                .FirstOrDefault(i => i.InterpreteId == id);
        }

        public InterpreteModel AtualizarInterprete(InterpreteModel interprete)
        {
            InterpreteModel interpreteDB =
                BuscarInterpretePorId(interprete.InterpreteId);

            if (interpreteDB == null)
                throw new Exception("Intérprete não encontrado");

            interpreteDB.NomeInterprete = interprete.NomeInterprete;
            interpreteDB.DtNascimento = DateTime.SpecifyKind(
                interprete.DtNascimento,
                DateTimeKind.Utc
            );
            interpreteDB.Telefone = interprete.Telefone;
            interpreteDB.Cpf = interprete.Cpf;
            interpreteDB.Email = interprete.Email;
            interpreteDB.PrecoDiaria = interprete.PrecoDiaria;

            _bancoContext.Interpretes.Update(interpreteDB);
            _bancoContext.SaveChanges();

            return interpreteDB;
        }

        public void ExcluirInterprete(int id)
        {
            InterpreteModel interpreteDB =
                _bancoContext.Interpretes
                .FirstOrDefault(i => i.InterpreteId == id);

            if (interpreteDB == null)
                throw new Exception("Intérprete não encontrado");

            _bancoContext.Interpretes.Remove(interpreteDB);
            _bancoContext.SaveChanges();
        }
    }
}