initCartHeader();

function initCartHeader() {
    var box1 = document.querySelector('.desktop-cart-wrapper .quickview-cart');
    var box2 = document.querySelector('.desktop-cart-wrapper1 .quickview-cart');

    function show1() {
        //console.log('showing');
        $(".cart-overlay1").addClass('open');
        box1.style.display = 'block';
    }

    function hide1() {
        //console.log('hiding');
        $(".cart-overlay1").removeClass('open');
        box1.style.display = 'none';
    }

    $(".desktop-cart-wrapper .btnCloseQVCart").click(function () {
        hide1();
    });

    var outside1 = function (event) {
        if (!box1.contains(event.target)) {
            hide1();
            //console.log('removing outside listener on document')
            this.removeEventListener(event.type, outside1);
        }
    }

    document.querySelector('.desktop-cart-wrapper > a').addEventListener('click', function (event) {
        event.preventDefault();
        event.stopPropagation();
        show1();
        document.addEventListener('click', outside1);
    });

    function show2() {
        //console.log('showing');
        $(".cart-overlay1").addClass('open');
        box2.style.display = 'block';
    }

    function hide2() {
        //console.log('hiding');
        $(".cart-overlay1").removeClass('open');
        box2.style.display = 'none';
    }

    $(".desktop-cart-wrapper1 .btnCloseQVCart").click(function () {
        hide2();
    });

    var outside2 = function (event) {
        if (!box2.contains(event.target)) {
            hide2();
            //console.log('removing outside listener on document')
            this.removeEventListener(event.type, outside2);
        }
    }

    document.querySelector('.desktop-cart-wrapper1 > a').addEventListener('click', function (event) {
        event.preventDefault();
        event.stopPropagation();
        show2();
        document.addEventListener('click', outside2);
    });

}