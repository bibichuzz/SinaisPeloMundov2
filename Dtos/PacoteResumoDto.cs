namespace SinaisPeloMundo.Dtos
{
    // Usado nas listas (Home e Catálogo)
    public class PacoteResumoDto
    {
        public int PacoteId { get; set; }
        public string Destino { get; set; } = "";
        public decimal Preco { get; set; }
        public string UrlImagem { get; set; } = "";

        public string? NomeHotel { get; set; }
        public DateTime? DataCheckin { get; set; }
        public DateTime? DataCheckout { get; set; }
    }
}