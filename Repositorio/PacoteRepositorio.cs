using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Dtos;
using SinaisPeloMundo.Helper;
using SinaisPeloMundo.Models;

namespace SinaisPeloMundo.Repositorio
{
    public class PacoteRepositorio : IPacoteRepositorio
    {
        private readonly BancoContext _bancoContext;
        public PacoteRepositorio(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }
        public List<PacoteModel> BuscarUltimosPacotes()
        {
            return _bancoContext.Pacotes
        .Include(p => p.ReservaHotel)
        .Include(p => p.Passagem)
        .Include(p => p.Interprete)
        .OrderByDescending(p => p.PacoteId)
        .Take(4)
        .ToList();
        }
        public List<PacoteModel> BuscarPacotes()
        {
            return _bancoContext.Pacotes
        .Include(p => p.ReservaHotel)
        .Include(p => p.Passagem)
        .Include(p => p.Interprete)
        .OrderByDescending(p => p.PacoteId)
        .ToList();
        }
        public List<PacoteModel> BuscarPacotesParaChatbot()
        {
            return _bancoContext.Pacotes
                .AsNoTracking()
                .Include(p => p.Passagem)
                .Include(p => p.ReservaHotel)
                .Include(p => p.Interprete)
                .ToList();
        }

        public List<PacoteChatbotDTO> BuscarPacotesParaRecomendacao()
        {
            var pacotes = BuscarPacotesParaChatbot();

            return pacotes.Select(p => new PacoteChatbotDTO
            {
                PacoteId = p.PacoteId,
                Destino = p.Destino,
                Preco = p.Preco,

                NomeHotel = p.ReservaHotel.NomeHotel,
                DataCheckin = p.ReservaHotel.DataCheckin,
                DataCheckout = p.ReservaHotel.DataCheckout,
                PrecoHotel = p.ReservaHotel.Preco,

                Transporte = p.Passagem.Transporte.ToString(),
                TipoPassagem = p.Passagem.TipoPassagem.ToString(),
                LocalPartida = p.Passagem.LocalPartida,
                LocalChegada = p.Passagem.LocalChegada,
                HorarioPartida = p.Passagem.HorarioPartida,
                HorarioChegada = p.Passagem.HorarioChegada,

                NomeInterprete = p.Interprete.NomeInterprete,
                PrecoDiariaInterprete = p.Interprete.PrecoDiaria,

                UrlDetalhes = $"/Pacote/DetalhesPacote/{p.PacoteId}"
            }).ToList();
        }

    }
}