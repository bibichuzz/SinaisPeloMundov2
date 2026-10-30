namespace SinaisPeloMundo.Dtos
{
    // Usado na tela de detalhes de um pacote
    public class PacoteDetalheDto
    {
        public int PacoteId { get; set; }
        public string Destino { get; set; } = "";
        public decimal Preco { get; set; }
        public string UrlImagem { get; set; } = "";

        public PassagemDto? Passagem { get; set; }
        public HotelDto? Hotel { get; set; }
        public InterpreteDto? Interprete { get; set; }
    }

    public class PassagemDto
    {
        public string Transporte { get; set; } = "";
        public string TipoPassagem { get; set; } = "";
        public decimal Preco { get; set; }
        public string LocalPartida { get; set; } = "";
        public DateTime HorarioPartida { get; set; }
        public string LocalChegada { get; set; } = "";
        public DateTime HorarioChegada { get; set; }
    }

    public class HotelDto
    {
        public string NomeHotel { get; set; } = "";
        public string EnderecoHotel { get; set; } = "";
        public DateTime DataCheckin { get; set; }
        public DateTime DataCheckout { get; set; }
        public decimal Preco { get; set; }
    }

    public class InterpreteDto
    {
        public string NomeInterprete { get; set; } = "";
    }
}