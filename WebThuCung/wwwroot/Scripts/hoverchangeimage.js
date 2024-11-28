$(document).on("mouseover", ".variant-image-loop", function (e) {
    e.preventDefault();
    var id = $(this).data('feature-image');
    var img = $(this).data('img');
    $('#' + id).attr('src', img);
});

$(document).on("mouseover", ".variant-image-loop1", function (a) {
    a.preventDefault();
    var id = $(this).data('feature-image1');
    var img = $(this).data('img1');
    $('#' + id + 'a').attr('src', img);
});

$(document).on("mouseover", ".variant-image-loop2", function (a) {
    a.preventDefault();
    var id = $(this).data('feature-image2');
    var img = $(this).data('img2');
    $('#' + id + 'b').attr('src', img);
});

$(document).on("mouseover", ".variant-image-loop3", function (a) {
    a.preventDefault();
    var id = $(this).data('feature-image3');
    var img = $(this).data('img3');
    $('#' + id + 'c').attr('src', img);
});

$(document).on("mouseover", ".variant-image-loop4", function (a) {
    a.preventDefault();
    var id = $(this).data('feature-image4');
    var img = $(this).data('img4');
    $('#' + id + 'd').attr('src', img);
});

$(document).on("mouseover", ".variant-image-loop5", function (a) {
    a.preventDefault();
    var id = $(this).data('feature-image5');
    var img = $(this).data('img5');
    $('#' + id + 'e').attr('src', img);
});