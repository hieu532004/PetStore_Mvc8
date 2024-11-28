const { type } = require("jquery");

isBool = true;
function showHidden() {
    if (isBol) {
        document.getElementById("matkhau").setAttribute("type", "text");
        isBool = false;
    }
    else {
        document.getElementById("matkhau").setAttribute("type", "password");
        isBool = true;
    }
}