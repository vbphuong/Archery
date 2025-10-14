$(document).ready(function () {
    // Hiệu ứng nhấp nháy nhẹ cho navbar brand
    $('.navbar-brand').on('mouseenter', function () {
        $(this).addClass('animate__animated animate__pulse');
    }).on('animationend', function () {
        $(this).removeClass('animate__animated animate__pulse');
    });

    // Hiệu ứng rung cho inbox khi có tin nhắn mới
    if ($('.badge.bg-danger').length > 0) {
        $('.inbox-icon').addClass('animate__animated animate__tada');
        setInterval(() => {
            $('.inbox-icon').toggleClass('animate__tada');
        }, 2000);
    }
});