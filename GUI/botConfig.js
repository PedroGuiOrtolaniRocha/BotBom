const getBtn = document.querySelector(".submit");
const getInput = document.querySelector(".botName");
const getPrefix = document.querySelector(".prefix");
function createBot(e) {
  const getTarget = e.target;
  const botName = getInput.value;
  const botPrefix = getPrefix.value;
  console.log(botName, botPrefix);

  console.log(getTarget);
}

getBtn.addEventListener("click", createBot);
