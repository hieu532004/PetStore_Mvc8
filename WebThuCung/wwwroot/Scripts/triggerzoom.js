$("document").ready(function () {
    $(function () {
        $(".js-qty__num").keypress(function (event) {
            if (event.which != 8 && event.which != 0 && (event.which < 48 || event.which > 57)) {
                return false;
            }
        });
    });
    if ($(window).width() > 768) {
        setTimeout(function () {
            $("#ProductThumbs .thumbnail-item:first-child a").trigger('click');
        }, 10);
    };
});