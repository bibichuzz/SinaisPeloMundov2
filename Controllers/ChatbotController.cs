
using Microsoft.AspNetCore.Mvc;
using SinaisPeloMundo.Models;
using SinaisPeloMundo.Services;
using SinaisPeloMundo.Repositorio;

namespace SinaisPeloMundo.Controllers
{
    [ApiController]
    [Route("api/chatbot")]
    public class ChatbotController : ControllerBase
    {
        private readonly GeminiService _geminiService;
        private readonly IPacoteRepositorio _pacoteRepositorio;

        public ChatbotController(
            GeminiService geminiService,
            IPacoteRepositorio pacoteRepositorio)
        {
            _geminiService = geminiService;
            _pacoteRepositorio = pacoteRepositorio;
        }


        [HttpPost("mensagem")]
        public async Task<IActionResult> EnviarMensagem(
            [FromBody] ChatbotMensagemModel mensagem)
        {
            if (mensagem == null ||
                string.IsNullOrWhiteSpace(mensagem.Mensagem))
            {
                return BadRequest(new
                {
                    resposta = "Digite uma mensagem."
                });
            }

            try
            {
                // Consulta os pacotes cadastrados no Supabase
                var pacotes = _pacoteRepositorio
                    .BuscarPacotesParaRecomendacao();

                // Obtém o histórico enviado pelo navegador
                var historico = mensagem.Historico ?? new List<ChatbotMensagemHistoricoModel>();

                // Limita o histórico às últimas 20 mensagens
                historico = historico
                    .Where(h =>
                        (h.Role == "user" || h.Role == "model") &&
                        !string.IsNullOrWhiteSpace(h.Conteudo))
                    .TakeLast(20)
                    .ToList();

                // Envia a mensagem, os pacotes e o histórico para o Gemini
                var resposta = await _geminiService
                    .EnviarMensagemAsync(mensagem.Mensagem, pacotes, historico);

                return Ok(new { resposta });
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Erro Gemini: " + ex);

                return StatusCode(502, new
                {
                    resposta = "Não consegui me conectar ao assistente. Tente novamente."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro Chatbot: " + ex);

                return StatusCode(500, new
                {
                    resposta = "Ocorreu um erro ao consultar os pacotes."
                });
            }
        }

    }
}