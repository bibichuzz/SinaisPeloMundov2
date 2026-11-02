
const chatForm = document.getElementById("chatForm");
const chatInput = document.getElementById("message");
const chatMessages = document.getElementById("chatMessages");
const chatApresentation = document.getElementById("chatApresentation");

// Histórico da conversa
let historico = [];

chatForm.addEventListener("submit", async function (event) {

    event.preventDefault();

    const mensagem = chatInput.value.trim();

    if (!mensagem) return;

    // Adiciona a mensagem do usuário na tela
    adicionarMensagem(mensagem, "user-messages");

    // Adiciona a mensagem ao histórico
    historico.push({
        role: "user",
        conteudo: mensagem
    });

    chatInput.value = "";

    // Esconde a apresentação inicial
    if (chatApresentation) {
        chatApresentation.style.display = "none";
    }

    // Indicador de carregamento
    const carregando = adicionarMensagem(
        "Digitando...",
        "bot-messages"
    );

    try {

        const resposta = await fetch("/api/chatbot/mensagem", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                mensagem: mensagem,
                historico: historico.slice(0, -1)
            })
        });

        const dados = await resposta.json();

        console.log("Status HTTP:", resposta.status);
        console.log("Resposta da API:", dados);

        if (!resposta.ok) {
            throw new Error(dados.resposta || "Erro na API.");
        }

        // Remove o indicador de carregamento
        carregando.remove();

        // Adiciona a resposta do Gemini na tela
        adicionarMensagem(
            dados.resposta,
            "bot-messages"
        );

        // Adiciona a resposta ao histórico
        historico.push({
            role: "model",
            conteudo: dados.resposta
        });

    } catch (erro) {

        carregando.textContent = "Erro ao enviar mensagem.";

        // Remove a mensagem que falhou do histórico
        historico.pop();

        console.error("Erro no chatbot:", erro);
    }

    chatMessages.scrollTop = chatMessages.scrollHeight;
});
function adicionarMensagem(texto, classe) {

    const mensagem = document.createElement("p");
    mensagem.classList.add(classe);

    const regex = /\/Pacote\/DetalhesPacote\/\d+/g;

    let ultimoIndice = 0;
    let resultado;

    while ((resultado = regex.exec(texto)) !== null) {

        // Texto antes do link
        mensagem.appendChild(
            document.createTextNode(
                texto.substring(ultimoIndice, resultado.index)
            )
        );

        // Cria o link
        const link = document.createElement("a");

        link.href = resultado[0];
        link.textContent = "detalhes do pacote";
        link.target = "_blank";
        link.rel = "noopener noreferrer";
        link.style.fontWeight = "bold";

        mensagem.appendChild(link);

        ultimoIndice = regex.lastIndex;
    }

    // Texto restante
    mensagem.appendChild(
        document.createTextNode(texto.substring(ultimoIndice))
    );

    chatMessages.appendChild(mensagem);

    chatMessages.scrollTop = chatMessages.scrollHeight;

    return mensagem;
}
