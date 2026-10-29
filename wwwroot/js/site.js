// ==============================
// Toggle Menu
// ==============================

const tiMenuToggle = document.getElementById("tiMenuToggle");
const tiNavbar = document.getElementById("tiNavbar");

if (tiMenuToggle && tiNavbar) {
    tiMenuToggle.addEventListener("click", () => {
        tiNavbar.classList.toggle("home-navbar-active");
    });
}


// ==============================
// AI Chat
// ==============================

const chatBotao = document.querySelector(".chat-botao");
const chatContainer = document.querySelector(".chat-container");
const closeChatBtn = document.querySelector(
    ".icon-header-chat .material-symbols-outlined:last-child"
);
const minimizeChatBtn = document.querySelector(
    ".icon-header-chat .material-symbols-outlined:first-child"
);

if (chatBotao && chatContainer) {
    chatBotao.addEventListener("click", () => {
        chatContainer.classList.toggle("chat-active");
    });
}

if (closeChatBtn && chatContainer) {
    closeChatBtn.addEventListener("click", () => {
        chatContainer.classList.remove("chat-active");
    });
}

if (minimizeChatBtn && chatContainer) {
    minimizeChatBtn.addEventListener("click", () => {
        chatContainer.classList.remove("chat-active");
    });
}


// ==============================
// Tema escuro
// ==============================

(function () {
    const htmlElement = document.documentElement;
    const themeToggle = document.getElementById("themeToggle");

    function applyTheme(theme) {

        if (theme === "dark") {
            htmlElement.setAttribute("data-theme", "dark");

            if (themeToggle) {
                themeToggle.textContent = "light_mode";
            }

        } else {
            htmlElement.removeAttribute("data-theme");

            if (themeToggle) {
                themeToggle.textContent = "dark_mode";
            }
        }
    }

    // Recupera o tema salvo
    const savedTheme = localStorage.getItem("theme") || "light";

    applyTheme(savedTheme);

    // Botão de trocar tema
    if (themeToggle) {

        themeToggle.addEventListener("click", () => {

            const isDark =
                htmlElement.getAttribute("data-theme") === "dark";

            const newTheme = isDark ? "light" : "dark";

            applyTheme(newTheme);

            localStorage.setItem("theme", newTheme);
        });
    }

})();
    
    // Botão de aumentar texto
    (function () {
    const htmlElement = document.documentElement;
    const increaseBtn = document.getElementById("textIncrease");
    const decreaseBtn = document.getElementById("textDecrease");

    const MIN_SCALE = 0.8;
    const MAX_SCALE = 1.4;
    const STEP = 0.1;

    function applyScale(scale) {
        const clamped = Math.min(MAX_SCALE, Math.max(MIN_SCALE, scale));
        htmlElement.style.setProperty("--font-scale", clamped);
        localStorage.setItem("fontScale", clamped);
    }

    // Aplica a escala salva assim que a página carrega
    const savedScale = parseFloat(localStorage.getItem("fontScale")) || 1;
    applyScale(savedScale);

    if (increaseBtn) {
        increaseBtn.addEventListener("click", () => {
            const current = parseFloat(localStorage.getItem("fontScale")) || 1;
            applyScale(current + STEP);
        });
    }

    if (decreaseBtn) {
        decreaseBtn.addEventListener("click", () => {
            const current = parseFloat(localStorage.getItem("fontScale")) || 1;
            applyScale(current - STEP);
        });
    }
})();