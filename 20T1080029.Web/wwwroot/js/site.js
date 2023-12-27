// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const scrollUp = document.querySelector('#scrollUp');
scrollUp.addEventListener('click', function () {
    window.scroll({
        top: 0,
        behavior: "smooth"
    });
});