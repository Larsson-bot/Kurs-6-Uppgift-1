window.exampleJsFunctions = {
    displayMessage: function (admincreateresponse) {
        document.getElementById('adminCreateResponse').innerText = admincreateresponse;
    }
    
};

window.onload = function () {
    window.localStorage.removeItem("AccessToken")
    return '';
}