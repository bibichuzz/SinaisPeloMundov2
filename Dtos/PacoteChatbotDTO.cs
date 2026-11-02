
namespace SinaisPeloMundo.Dtos
{
    public class PacoteChatbotDTO
    {
        public int PacoteId { get; set; }

        public string Destino { get; set; }

        public decimal Preco { get; set; }

        public string NomeHotel { get; set; }

        public DateTime DataCheckin { get; set; }

        public DateTime DataCheckout { get; set; }

        public decimal PrecoHotel { get; set; }

        public string Transporte { get; set; }

        public string TipoPassagem { get; set; }

        public string LocalPartida { get; set; }

        public string LocalChegada { get; set; }

        public DateTime HorarioPartida { get; set; }

        public DateTime HorarioChegada { get; set; }

        public string NomeInterprete { get; set; }

        public decimal PrecoDiariaInterprete { get; set; }

        public int DuracaoDias =>
            (DataCheckout.Date - DataCheckin.Date).Days;
        public string UrlDetalhes { get; set; } = string.Empty;
    }
}
