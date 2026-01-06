// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $(".hero__categories__all").on("click", function () {
        // chỉ tìm ul gốc, không dính slicknav
        $(this).siblings("ul.hero__categories__list")
            .stop(true, true)
            .slideToggle(300);
    });
});

