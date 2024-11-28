//document.addEventListener("DOMContentLoaded", function () {
//    const menuItems = document.querySelectorAll(".profile-menu-item");

//    menuItems.forEach(item => {
//        item.addEventListener("click", function () {
//            // Xóa lớp 'active' khỏi tất cả các mục
//            menuItems.forEach(i => i.classList.remove("active"));

//            // Thêm lớp 'active' cho mục được chọn
//            this.classList.add("active");
//        });
//    });
//});

$(document).on('click', '.avatar_img', function () {
    var profileMenu = $('.profileavatar');
    if (profileMenu.css('display') === 'none') {
        profileMenu.css('display', 'block'); // Hiển thị menu
    } else {
        profileMenu.css('display', 'none'); // Ẩn menu
    }
});

// Ẩn menu khi nhấp ra ngoài menu
$(document).click(function (e) {
    if (!$(e.target).closest('.wrap_login').length) {
        $('.profile').hide(); // Ẩn danh sách khi nhấp ra ngoài avatar
    }
});

(function ($) {

  "use strict";

  var initPreloader = function() {
    $(document).ready(function($) {
    var Body = $('body');
        Body.addClass('preloader-site');
    });
    $(window).load(function() {
        $('.preloader-wrapper').fadeOut();
        $('body').removeClass('preloader-site');
    });
  }
    //$(document).ready(function () {
    //    // Xử lý sự kiện khi nhấp vào mục màu
    //    $('#colorList').on('click', '.color-item', function (e) {
    //        e.preventDefault(); // Ngăn hành động mặc định của thẻ <a>

    //        // Xóa lớp 'color-selected' khỏi tất cả các mục
    //        $('.color-item').removeClass('color-selected');

    //        // Thêm lớp 'color-selected' vào mục đã nhấp
    //        $(this).addClass('color-selected');
    //    });

    //    $('#sizeList').on('click', '.size-item', function (e) {
    //        e.preventDefault(); // Ngăn hành động mặc định của thẻ <a>

    //        // Xóa lớp 'color-selected' khỏi tất cả các mục
    //        $('.size-item').removeClass('color-selected');

    //        // Thêm lớp 'color-selected' vào mục đã nhấp
    //        $(this).addClass('color-selected');
    //    });
    //});
  // init Chocolat light box
	var initChocolat = function() {
		Chocolat(document.querySelectorAll('.image-link'), {
		  imageSize: 'contain',
		  loop: true,
		})
	}

  var initSwiper = function() {

    var swiper = new Swiper(".main-swiper", {
      speed: 500,
      pagination: {
        el: ".swiper-pagination",
        clickable: true,
      },
    });

    var bestselling_swiper = new Swiper(".bestselling-swiper", {
      slidesPerView: 4,
      spaceBetween: 30,
      speed: 500,
      breakpoints: {
        0: {
          slidesPerView: 1,
        },
        768: {
          slidesPerView: 3,
        },
        991: {
          slidesPerView: 4,
        },
      }
    });

    var testimonial_swiper = new Swiper(".testimonial-swiper", {
      slidesPerView: 1,
      speed: 500,
      pagination: {
        el: ".swiper-pagination",
        clickable: true,
      },
    });

    var products_swiper = new Swiper(".products-carousel", {
      slidesPerView: 4,
      spaceBetween: 30,
      speed: 500,
      breakpoints: {
        0: {
          slidesPerView: 1,
        },
        768: {
          slidesPerView: 3,
        },
        991: {
          slidesPerView: 4,
        },

      }
    });

  }

  var initProductQty = function(){

    $('.product-qty').each(function(){

      var $el_product = $(this);
      var quantity = 0;

      $el_product.find('.quantity-right-plus').click(function(e){
          e.preventDefault();
          var quantity = parseInt($el_product.find('#quantity').val());
          $el_product.find('#quantity').val(quantity + 1);
      });

      $el_product.find('.quantity-left-minus').click(function(e){
          e.preventDefault();
          var quantity = parseInt($el_product.find('#quantity').val());
          if(quantity>0){
            $el_product.find('#quantity').val(quantity - 1);
          }
      });

    });

  }

  // init jarallax parallax
  var initJarallax = function() {
    jarallax(document.querySelectorAll(".jarallax"));

    jarallax(document.querySelectorAll(".jarallax-keep-img"), {
      keepImg: true,
    });
  }

  // document ready
  $(document).ready(function() {
    
    initPreloader();
    initSwiper();
    initProductQty();
    initJarallax();
    initChocolat();

        // product single page
        var thumb_slider = new Swiper(".product-thumbnail-slider", {
          spaceBetween: 8,
          slidesPerView: 3,
          freeMode: true,
          watchSlidesProgress: true,
        });
    
        var large_slider = new Swiper(".product-large-slider", {
          spaceBetween: 10,
          slidesPerView: 1,
          effect: 'fade',
          thumbs: {
            swiper: thumb_slider,
          },
        });

    window.addEventListener("load", (event) => {
      //isotope
      $('.isotope-container').isotope({
        // options
        itemSelector: '.item',
        layoutMode: 'masonry'
      });


      var $grid = $('.entry-container').isotope({
        itemSelector: '.entry-item',
        layoutMode: 'masonry'
      });


      // Initialize Isotope
      var $container = $('.isotope-container').isotope({
        // options
        itemSelector: '.item',
        layoutMode: 'masonry'
      });

      $(document).ready(function () {
        //active button
        $('.filter-button').click(function () {
          $('.filter-button').removeClass('active');
          $(this).addClass('active');
        });
      });

      // Filter items on button click
      $('.filter-button').click(function () {
        var filterValue = $(this).attr('data-filter');
        if (filterValue === '*') {
          // Show all items
          $container.isotope({ filter: '*' });
        } else {
          // Show filtered items
          $container.isotope({ filter: filterValue });
        }
      });

    });

  }); // End of a document

})(jQuery);