function getContent(e) {
  const getUrl = document.querySelector(".url_value");
  const getPrefix = document.querySelector(".prefix");
  const getAqui = document.querySelector(".aqui");
  const getUrlValue = getUrl.value;
  const getPrefixValue = getPrefix.value;
  const target = e.target;

  const object = { url: getUrlValue, prefix: getPrefixValue };

  if (target.classList.contains("submit")) {
    if (!getUrlValue) return;
    const getJson = JSON.stringify(object);
    console.log(getJson);
    console.log(getUrlValue);
    console.log(getPrefixValue);
  }
}
document.addEventListener("click", getContent);
