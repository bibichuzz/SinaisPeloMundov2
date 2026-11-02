
namespace SinaisPeloMundo.Models
{
    public class ChatbotMensagemModel
    {
        public string Mensagem { get; set; } = string.Empty;

        public List<ChatbotMensagemHistoricoModel> Historico { get; set; } = new();
    }

    public class ChatbotMensagemHistoricoModel
    {
        public string Role { get; set; } = string.Empty;

        public string Conteudo { get; set; } = string.Empty;
    }
}
