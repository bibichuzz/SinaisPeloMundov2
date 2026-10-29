using SinaisPeloMundo.Models;

namespace SinaisPeloMundo.Repositorio
{
    public interface IAdminRepositorio
    {
        List<PacoteModel> BuscarPacotes();

        List<PassagemModel> BuscarPassagens();
        List<PassagemModel> BuscarPassagensDisponiveis();
        List<PassagemModel> BuscarPassagensDisponiveisEditar(int id);

        List<ReservaHotelModel> BuscarReservasHotel();
        List<ReservaHotelModel> BuscarReservasHotelDisponiveis();
        List<ReservaHotelModel> BuscarReservasHotelDisponiveisEditar(int id);

        List<InterpreteModel> BuscarInterpretes();

        PacoteModel AdicionarPacote(PacoteModel pacote);
        public PacoteModel BuscarPacotePorId(int id);
        public PacoteModel AtualizarPacote(PacoteModel pacote);
        public void ExcluirPacote(int id);
        public PassagemModel AdicionarPassagem(PassagemModel passagem);
        public PassagemModel BuscarPassagemPorId(int id);
        public PassagemModel AtualizarPassagem(PassagemModel passagem);
        public void ExcluirPassagem(int id);
        public ReservaHotelModel AdicionarHotel(ReservaHotelModel hotel);
        public ReservaHotelModel BuscarHotelPorId(int id);
        public ReservaHotelModel AtualizarHotel(ReservaHotelModel hotel);
        public void ExcluirHotel(int id);
        public InterpreteModel AdicionarInterprete(InterpreteModel interprete);
        public InterpreteModel BuscarInterpretePorId(int id);
        public InterpreteModel AtualizarInterprete(InterpreteModel interprete);
        public void ExcluirInterprete(int id);
    }
}