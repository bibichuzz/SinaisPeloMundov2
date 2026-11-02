using SinaisPeloMundo.Dtos;
using SinaisPeloMundo.Models;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace SinaisPeloMundo.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        // Modelo utilizado pela API
        private const string Modelo = "gemini-3.5-flash-lite";

        public GeminiService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;

            _apiKey = configuration["Gemini:ApiKey"]
                ?? throw new InvalidOperationException(
                    "A chave da API Gemini não foi configurada.");
        }


        public async Task<string> EnviarMensagemAsync(
            string mensagem,
            List<PacoteChatbotDTO> pacotes,
            List<ChatbotMensagemHistoricoModel> historico)
        {
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{Modelo}:generateContent";

            var instrucoes = """
        Você é o Safe, assistente virtual oficial do Sinais Pelo Mundo,
        uma agência de turismo especializada em viagens acessíveis
        para pessoas surdas.

        
        PERSONALIDADE:
        - Você é o Safe, assistente virtual do Sinais Pelo Mundo.
        - Seja simpático, acolhedor, educado e prestativo.
        - Converse sempre em português brasileiro.
        - Utilize linguagem natural, simples e objetiva.
        - Utilize emojis moderadamente.

        COMPORTAMENTO NA CONVERSA:
        - NÃO se apresente ao usuário.
        - NÃO diga seu nome ou explique sua função.
        - NÃO comece as respostas com saudações como "Olá", "Oi" ou "Seja bem-vindo".
        - NÃO repita apresentações ou introduções.
        - Vá diretamente ao assunto da mensagem do usuário.
        - Mantenha uma conversa contínua e natural.
        - Considere o histórico e as informações já fornecidas pelo usuário.

        ATENDIMENTO:
        - Se o usuário ainda não informou o destino, pergunte qual destino deseja conhecer.
        - Depois, pergunte sobre o período da viagem, duração e orçamento, conforme necessário.
        - Faça perguntas de maneira gradual, sem apresentar um questionário inteiro de uma só vez.
        - Não repita perguntas que o usuário já respondeu.
        - Considere as preferências de transporte e hospedagem.
        

        RECOMENDAÇÕES:
        - Analise os pacotes disponíveis nos dados recebidos.
        - Compare destinos, datas, duração e preços.
        - Priorize os pacotes que correspondam às preferências do usuário.
        - Sugira alternativas semelhantes quando não houver correspondência exata.
        - Explique por que o pacote indicado é uma boa opção.

        REGRAS:
        - Nunca invente pacotes, destinos, hotéis, preços ou datas.
        - Utilize exclusivamente os dados fornecidos pelo sistema.
        - Nunca afirme que existem vagas disponíveis sem confirmação.
        - Se nenhum pacote for adequado, informe isso com transparência.
        - Se não houver pacotes cadastrados, explique a situação.

        ACESSIBILIDADE:
        - Valorize a comunicação em Libras.
        - Destaque a presença de intérpretes.
        - Priorize autonomia, conforto e inclusão.

        FORMATO:
        - Apresente as recomendações de maneira organizada.
        - Destaque destino, período, duração e preço.
        - Pergunte se o cliente deseja conhecer mais detalhes.
        - NÃO utilize Markdown.
        - NÃO utilize asteriscos para negrito ou itálico.
        - NÃO utilize hashtags para títulos.
        - NÃO utilize listas com marcadores ou numeração.
        - NÃO utilize crases ou blocos de código.
        - NÃO utilize tabelas.
        - Escreva de maneira natural, como em uma conversa de chat.
        - Separe informações em parágrafos mais curtos quando necessário.

        LINKS DOS PACOTES:
        - Sempre que recomendar um pacote, informe seu link de acesso.
        - Utilize exclusivamente a URL fornecida pelo sistema no campo UrlDetalhes.
        - Nunca invente ou modifique links.
        - Se a URL não estiver disponível, não crie um link.
        """;

            var dadosPacotes = JsonSerializer.Serialize(pacotes);

            var contents = new List<object>();

            // Adiciona as mensagens anteriores
            foreach (var item in historico)
            {
                if (item.Role != "user" && item.Role != "model")
                    continue;

                if (string.IsNullOrWhiteSpace(item.Conteudo))
                    continue;

                contents.Add(new
                {
                    role = item.Role,
                    parts = new[]
                    {
            new { text = item.Conteudo }
        }
                });
            }

            // Adiciona a mensagem atual e os dados dos pacotes
            contents.Add(new
            {
                role = "user",
                parts = new[]
                {
        new
        {
            text = $"""
                DADOS DOS PACOTES CADASTRADOS NO SISTEMA:
                {dadosPacotes}

                MENSAGEM ATUAL DO USUÁRIO:
                {mensagem}
                """
        }
    }
            });

            var corpo = new
            {
                system_instruction = new
                {
                    parts = new[]
                    {
                new { text = instrucoes }
            }
                },

                contents = contents,

                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 800
                }
            };

            using var requisicao = new HttpRequestMessage(
                HttpMethod.Post, url);

            requisicao.Headers.Add("x-goog-api-key", _apiKey);
            requisicao.Content = JsonContent.Create(corpo);

            using var resposta = await _httpClient.SendAsync(requisicao);

            var conteudo = await resposta.Content.ReadAsStringAsync();

            if (!resposta.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Erro na API Gemini ({(int)resposta.StatusCode}): {conteudo}");
            }

            using var documento = JsonDocument.Parse(conteudo);

            var texto = documento.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return texto ?? "Não consegui gerar uma resposta.";
        }

    }
}