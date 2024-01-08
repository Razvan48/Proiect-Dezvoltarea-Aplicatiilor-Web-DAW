// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function storeScrollPosition() {
    var scrollPosition = window.scrollY;
    sessionStorage.setItem("scrollPosition", window.scrollY);
}

function getStoredScrollPosition() {
    return parseInt(sessionStorage.getItem("scrollPosition")) || 0;
}

window.onload = function () {
    window.scrollTo(0, getStoredScrollPosition());

    console.log("Scroll Position:", getStoredScrollPosition());
    sessionStorage.setItem("scrollPosition", 0); // reseteaza
    console.log("Scroll Position:", getStoredScrollPosition());
};

