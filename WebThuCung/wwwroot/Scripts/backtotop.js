jQuery(document).ready(function () {
    var offset = 220;
    var duration = 500;
    jQuery(window).scroll(function () {
        if (jQuery(this).scrollTop() > offset) {
            jQuery('#back-to-top').fadeIn(duration);
        } else {
            jQuery('#back-to-top').fadeOut(duration);
        }
    });

    jQuery('#back-to-top').click(function (event) {
        event.preventDefault();
        jQuery('html, body').animate({
            scrollTop: 0
        }, duration);
        return false;
    });

    window.onscroll = changePos;

    function changePos() {
        var header = $("#header");
        var headerheight = $("#header").height();
        if (window.pageYOffset > headerheight) {
            header.addClass('scrolldown');
        } else {
            header.removeClass('scrolldown');
        }
    }
});