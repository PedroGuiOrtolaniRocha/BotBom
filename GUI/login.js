const getClick = document.querySelector(".submit");
const fs = require("fs");

function getContent(e) {
  const getPassword = document.querySelector(".password");
  const getEmail = document.querySelector(".email");
  // const getPrefix = document.querySelector(".prefix");
  const getPasswordValue = getPassword.value;
  const getEmailValue = getEmail.value;
  const target = e.target;
  return aa(getEmailValue, getPasswordValue);

  function aa(email, password) {
    console.log(email, password);
    if (target.classList.contains("submit")) {
      if (!getPasswordValue || !getEmailValue) return;
      readConfigFile();
    }

    // Função para ler e processar o conteúdo do arquivo
    function readConfigFile() {
      fs.readFile("../config.json", "utf8", (err, data) => {
        if (err) {
          console.error("Erro ao ler o arquivo:", err);
          return;
        }
        // Aqui você pode processar o conteúdo do arquivo
        console.log(data); // Exibe o conteúdo do arquivo
        const config = JSON.parse(data);
        console.log(config); // Exibe o objeto JavaScript parseado do JSON
      });
    }

    //window.location.href = "botConfig.html";
  }
}

getClick.addEventListener("click", getContent);
