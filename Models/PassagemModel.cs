using System.ComponentModel.DataAnnotations;

namespace SinaisPeloMundo.Models
{
    public class PassagemModel
    {
        [Key]
        public int PassagemId { get; set; }

        [Required(ErrorMessage = "Selecione o tipo de transporte.")]
        public TipoTransporte Transporte { get; set; }

        [Required(ErrorMessage = "Selecione o tipo de passagem.")]
        public TipoPassagem TipoPassagem { get; set; }

        [Required(ErrorMessage = "Informe o preço.")]
        [Range(0.01, 999999,
            ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Informe a poltrona.")]
        [Range(1, 9999,
            ErrorMessage = "Informe uma poltrona válida.")]
        public int Poltrona { get; set; }

        [Required(ErrorMessage = "Informe a placa do transporte.")]
        [MaxLength(20,
            ErrorMessage = "A placa pode ter no máximo 20 caracteres.")]
        public string PlacaTransporte { get; set; }

        [Required(ErrorMessage = "Informe o horário de partida.")]
        public DateTime HorarioPartida { get; set; }

        [Required(ErrorMessage = "Informe o local de partida.")]
        [MaxLength(50,
            ErrorMessage = "O local de partida pode ter no máximo 50 caracteres.")]
        public string LocalPartida { get; set; }

        [Required(ErrorMessage = "Informe o horário de chegada.")]
        public DateTime HorarioChegada { get; set; }

        [Required(ErrorMessage = "Informe o local de chegada.")]
        [MaxLength(50,
            ErrorMessage = "O local de chegada pode ter no máximo 50 caracteres.")]
        public string LocalChegada { get; set; }

        public PacoteModel? Pacote { get; set; }
    }
}