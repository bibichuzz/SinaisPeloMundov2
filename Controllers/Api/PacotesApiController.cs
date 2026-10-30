using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Data;
using SinaisPeloMundo.Dtos;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Repositorio;

namespace SinaisPeloMundo.Controllers.Api
{
    [ApiController]
    [Route("api/pacotes")]
    public class PacotesApiController : ControllerBase
    {
        private readonly IPacoteRepositorio _pacoteRepositorio;
        private readonly BancoContext _bancoContext;

        public PacotesApiController(
            IPacoteRepositorio pacoteRepositorio,
            BancoContext bancoContext)
        {
            _pacoteRepositorio = pacoteRepositorio;
            _bancoContext = bancoContext;
        }

        // GET api/pacotes/destaques  (equivale a Home/Index)
        [HttpGet("destaques")]
        public IActionResult Destaques()
        {
            var pacotes = _pacoteRepositorio
                .BuscarUltimosPacotes()
                .Select(ParaResumo)
                .ToList();

            return Ok(pacotes);
        }

        // GET api/pacotes  (equivale a Home/Catalogo)
        [HttpGet]
        public IActionResult Listar()
        {
            var pacotes = _pacoteRepositorio
                .BuscarPacotes()
                .Select(ParaResumo)
                .ToList();

            return Ok(pacotes);
        }

        // GET api/pacotes/5  (equivale a Pacote/DetalhesPacote)
        [HttpGet("{id:int}")]
        public IActionResult Detalhes(int id)
        {
            var pacote = _bancoContext.Pacotes
                .AsNoTracking()
                .Include(p => p.Passagem)
                .Include(p => p.ReservaHotel)
                .Include(p => p.Interprete)
                .FirstOrDefault(p => p.PacoteId == id);

            if (pacote == null)
                return NotFound(new { mensagem = "Pacote não encontrado." });

            return Ok(ParaDetalhe(pacote));
        }

        // ---------- conversões model -> DTO ----------

        private static PacoteResumoDto ParaResumo(PacoteModel p) => new()
        {
            PacoteId = p.PacoteId,
            Destino = p.Destino,
            Preco = p.Preco,
            UrlImagem = p.UrlImagem,
            NomeHotel = p.ReservaHotel?.NomeHotel,
            DataCheckin = p.ReservaHotel?.DataCheckin,
            DataCheckout = p.ReservaHotel?.DataCheckout
        };

        private static PacoteDetalheDto ParaDetalhe(PacoteModel p) => new()
        {
            PacoteId = p.PacoteId,
            Destino = p.Destino,
            Preco = p.Preco,
            UrlImagem = p.UrlImagem,

            Passagem = p.Passagem == null ? null : new PassagemDto
            {
                Transporte = p.Passagem.Transporte.ToString(),
                TipoPassagem = p.Passagem.TipoPassagem.ToString(),
                Preco = p.Passagem.Preco,
                LocalPartida = p.Passagem.LocalPartida,
                HorarioPartida = p.Passagem.HorarioPartida,
                LocalChegada = p.Passagem.LocalChegada,
                HorarioChegada = p.Passagem.HorarioChegada
            },

            Hotel = p.ReservaHotel == null ? null : new HotelDto
            {
                NomeHotel = p.ReservaHotel.NomeHotel,
                EnderecoHotel = p.ReservaHotel.EnderecoHotel,
                DataCheckin = p.ReservaHotel.DataCheckin,
                DataCheckout = p.ReservaHotel.DataCheckout,
                Preco = p.ReservaHotel.Preco
            },

            Interprete = p.Interprete == null ? null : new InterpreteDto
            {
                NomeInterprete = p.Interprete.NomeInterprete
            }
        };
    }
}
