/*load ajax once time*/
$(".load-ajax").click(function(){
	var _this = $(this);
	var _url = $(this).data('url');
	var _container = $(this).data('container');
	$(_container).append('<div class="loading-ajax-once-time">Đang cập nhật dữ liêu...</div>')
	$.ajax({
		url: _url,
		async : false,
		type:'GET',
		success:function(data){
			$(_container).remove('.loading-ajax-once-time');
			$(_container).html(data);
			_this.removeClass('load-ajax');
		},
		complete:function() {
		}
	});
});

$(document).on('click','.desktop-cart-wrapper .quickview-cart ul li .cart__remove',  function(e){
	e.preventDefault() ;
	var params = {
		type: 'POST',
		url: '/cart/change.js',
		data: $(this).attr('href').split('?')[1],
		async : false,
		success: function() {
			$.ajax({
				type: 'GET',
				url: '/cart?view=desktopheader',
				async: true,
				success: function(data){
					updateCart();
					$('.desktop-cart-wrapper .quickview-cart').html(data);
					initCartHeader();
				}
			})
		},
		error: function(XMLHttpRequest, textStatus) {
			Haravan.onError(XMLHttpRequest, textStatus);
		}
	};
	jQuery.ajax(params);	
})

$(document).on('click','.desktop-cart-wrapper1 .quickview-cart ul li .cart__remove',  function(e){
	e.preventDefault() ;
	var params = {
		type: 'POST',
		url: '/cart/change.js',
		data: $(this).attr('href').split('?')[1],
		async : false,
		success: function() {
			$.ajax({
				type: 'GET',
				url: '/cart?view=desktopheader',
				async: true,
				success: function(data){
					updateCart();
					$('.desktop-cart-wrapper1 .quickview-cart').html(data);
					initCartHeader();
				}
			})
		},
		error: function(XMLHttpRequest, textStatus) {
			Haravan.onError(XMLHttpRequest, textStatus);
		}
	};
	jQuery.ajax(params);	
})


/* Add to cart, Buy now*/
$(document).on("click",".buy-now", function(e){
	e.preventDefault() ;
	var params = {
		type: 'POST',
		url: '/cart/add.js',
		async : false,
		data: {quantity:1,id:$(this).data('id')},
		dataType: 'json',
		success: function(line_item) {
			window.location = '/checkout';
		},
		error: function(XMLHttpRequest, textStatus) {
			Haravan.onError(XMLHttpRequest, textStatus);
		}
	};
	jQuery.ajax(params);
});

$(document).on("click",".add-to-cart", function(e){
	e.preventDefault() ;
	var params = {
		type: 'POST',
		url: '/cart/add.js',
		async : false,
		data: {quantity:1,id:$(this).data('id')},
		dataType: 'json',
		success: function(line_item) {
			updateCart();
			updateCartModal();
			/*jQuery('html, body').animate({
				scrollTop: 0
			}, 500);*/
		},
		error: function(XMLHttpRequest, textStatus) {
			Haravan.onError(XMLHttpRequest, textStatus);
		}
	};
	jQuery.ajax(params);
});

/*buy now for product page*/
$('#buy-now').click(function(e){	
	e.preventDefault() ;
	var qty = $('#Quantity').val();
	var params = {
		type: 'POST',
		url: '/cart/add.js',
		async : false,
		data: {quantity:qty,id:$('#productSelect').val()},
		dataType: 'json',
		success: function(line_item) {
			window.location = '/checkout';
		},
		error: function(XMLHttpRequest, textStatus) {
			Haravan.onError(XMLHttpRequest, textStatus);
		}
	};
	jQuery.ajax(params);
});

/*update cart count*/
function updateCart(){
	$.ajax({
		type: 'GET',
		url: '/cart?view=count',
		async: true,
		success: function(data){
			$('.desktop-cart-wrapper a .hd-cart-count').html(parseInt(data));
			$('.desktop-cart-wrapper1 a .hd-cart-count').html(parseInt(data));
			$('.mobile-nav-item a .number').html(parseInt(data));
		}
	})
	$.ajax({
		type: 'GET',
		url: '/cart?view=desktopheader',
		async: true,
		success: function(data){
			$('.desktop-cart-wrapper .quickview-cart').html(data);
			$('.desktop-cart-wrapper1 .quickview-cart').html(data);
			initCartHeader();
		}
	})
}

function updateCartModal(){
	$.ajax({
		type: 'GET',
		url: '/cart?view=addcomplete',
		async : false,
		success: function(data) {
			//do html vao day
			$('#modalAddComplete').html(data);
			$('#modalAddComplete').css('display','block');
			setTimeout(function(){
				$('.modalAddComplete-content').addClass('show');
			}, 100)
		}
	})
}

function setCookie(cname, cvalue, exdays) {
	var d = new Date();
	d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
	var expires = "expires=" + d.toUTCString();
	document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
	var name = cname + "=";
	var ca = document.cookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') {
			c = c.substring(1);
		}
		if (c.indexOf(name) == 0) {
			return c.substring(name.length, c.length);
		}
	}
	return "";
}

function checkCookie() {
	var user = getCookie("username");
	if (user != "") {
		alert("Welcome again " + user);
	} else {
		user = prompt("Please enter your name:", "");
		if (user != "" && user != null) {
			setCookie("username", user, 365);
		}
	}
}

/* Owl carousel */
var navLeftText = '<i class="fa fa-angle-right" aria-hidden="true"></i>';
var navRightText = '<i class="fa fa-angle-left" aria-hidden="true"></i>';

$(function(){

	$(".owl-carousel.owl-enable").each(function(){
		var config = {
			margin: 10,
			lazyLoad: true,
			navigationText: [navLeftText, navRightText]
		}; 

		var owl = $(this);
		if( $(this).data('slide') == 1 ){
			config.singleItem = true;
		}else {
			config.items = $(this).data( 'items' );
		}
		if ($(this).data('items')) {
			config.itemsDesktop = $(this).data('items');
		}
		if ($(this).data('desktop')) {
			config.itemsDesktop = $(this).data('desktop');
		}
		if ($(this).data('desktopsmall')) {
			config.itemsDesktopSmall = $(this).data('desktopsmall');
		}
		if ($(this).data('tablet')) {
			config.itemsTablet = $(this).data('tablet');
		}
		if ($(this).data('tabletsmall')) {
			config.itemsTabletSmall = $(this).data('tabletsmall');
		}
		if ($(this).data('mobile')) {
			config.itemsMobile = $(this).data('mobile');
		}
		if ($(this).data('autoplay')) {
			config.autoPlay = $(this).data('autoplay');
		}
		if ($(this).data('nav')) {
			config.navigation = $(this).data('nav');
		}

		$(this).owlCarousel( config );
	});
})



/* variant click */

function convertToSlug(str) {

	str= str.toLowerCase();  
	str= str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g,"a");  
	str= str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g,"e");  
	str= str.replace(/ì|í|ị|ỉ|ĩ/g,"i");  
	str= str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g,"o");  
	str= str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g,"u");  
	str= str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g,"y");  
	str= str.replace(/đ/g,"d");  
	str= str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_/g,"-"); 
	str= str.replace(/-+-/g,"-");
	str= str.replace(/^\-+|\-+$/g,"");  
	return str;  
} 

 var swatch_size = 0;
 jQuery(document).ready(function(){

	 jQuery('#productQuickView').on('click','input.input-quickview', function(e) {  
		 e.preventDefault();
		 console.log('quickviewXXX');
		 swatch_size = parseInt($('#productQuickView .select-swatch').children().size());
		 var $this = $(this);
		 var _available = '';
		 $this.parent().siblings().find('label').removeClass('sd');
		 $this.next().addClass('sd');
		 var name = $this.attr('name');
		 var value = $this.val();
		 $('#productQuickView select[data-option='+name+']').val(value).trigger('change');
		 if($(this).data('img-src')){
			 var img_ = $(this).data('img-src');
			 $('#productQuickView .product-single__thumbnail[href="'+img_+'"]').trigger('click');
		 }
		 if(swatch_size == 2){
			 if(name.indexOf('1') != -1){
				 $('#variant-swatch-1-quickview .swatch-element').find('input').prop('disabled', false);
				 $('#variant-swatch-2-quickview .swatch-element').find('input').prop('disabled', false);
				 $('#variant-swatch-1-quickview .swatch-element label').removeClass('sd');
				 $('#variant-swatch-1-quickview .swatch-element').removeClass('soldout');
				 $('#productQuickView .selector-wrapper .single-option-selector').eq(1).find('option').each(function(){
					 var _tam = $(this).val();
					 $(this).parent().val(_tam).trigger('change');
					 if(check_variant_quickview){
						 if(_available == '' ){
							 _available = _tam;
						 }
					 }else{
						 $('#variant-swatch-1-quickview .swatch-element[data-value="'+_tam+'"]').addClass('soldout');
						 $('#variant-swatch-1-quickview .swatch-element[data-value="'+_tam+'"]').find('input').prop('disabled', true);
					 }
				 })
				 $('#productQuickView .selector-wrapper .single-option-selector').eq(1).val(_available).trigger('change');
				 $('#variant-swatch-1-quickview .swatch-element[data-value="'+_available+'"] label').addClass('sd');
			 }
		 }else if (swatch_size == 3){
			 var _count_op2 = $('#variant-swatch-1-quickview .swatch-element').size();
			 var _count_op3 = $('#variant-swatch-2-quickview .swatch-element').size();
			 if(name.indexOf('1') != -1){
				 $('#variant-swatch-1-quickview .swatch-element').find('input').prop('disabled', false);
				 $('#variant-swatch-2-quickview .swatch-element').find('input').prop('disabled', false);
				 $('#variant-swatch-1-quickview .swatch-element label').removeClass('sd');
				 $('#variant-swatch-1-quickview .swatch-element').removeClass('soldout');
				 $('#variant-swatch-2-quickview .swatch-element label').removeClass('sd');
				 $('#variant-swatch-2-quickview .swatch-element').removeClass('soldout');
				 var _avi_op1 = '';
				 var _avi_op2 = '';
				 $('#variant-swatch-1-quickview .swatch-element').each(function(ind,value){
					 var _key = $(this).data('value');
					 var _unavi = 0;
					 $('#productQuickView .single-option-selector').eq(1).val(_key).change();
					 $('#variant-swatch-2-quickview .swatch-element label').removeClass('sd');
					 $('#variant-swatch-2-quickview .swatch-element').removeClass('soldout');
					 $('#variant-swatch-2-quickview .swatch-element').find('input').prop('disabled', false);
					 $('#variant-swatch-2-quickview .swatch-element').each(function(i,v){
						 var _val = $(this).data('value');
						 $('#productQuickView .single-option-selector').eq(2).val(_val).change();
						 if(check_variant == true){
							 if(_avi_op1 == ''){
								 _avi_op1 = _key;
							 }
							 if(_avi_op2 == ''){
								 _avi_op2 = _val;
							 }
							 //console.log(_avi_op1 + ' -- ' + _avi_op2)
						 }else{
							 _unavi += 1;
						 }
					 })
					 if(_unavi == _count_op3){
						 $('#variant-swatch-1-quickview .swatch-element[data-value = "'+_key+'"]').addClass('soldout');
						 setTimeout(function(){
							 $('#variant-swatch-1-quickview .swatch-element[data-value = "'+_key+'"] input').attr('disabled','disabled');
						 })
					 }
				 })
				 $('#variant-swatch-1-quickview .swatch-element[data-value="'+_avi_op1+'"] input').click();
			 }
			 else if(name.indexOf('2') != -1){
				 $('#variant-swatch-2-quickview .swatch-element label').removeClass('sd');
				 $('#variant-swatch-2-quickview .swatch-element').removeClass('soldout');
				 $('#productQuickView .selector-wrapper .single-option-selector').eq(2).find('option').each(function(){
					 var _tam = $(this).val();
					 $(this).parent().val(_tam).trigger('change');
					 if(check_variant_quickview){
						 if(_available == '' ){
							 _available = _tam;
						 }
					 }else{
						 $('#variant-swatch-2-quickview .swatch-element[data-value="'+_tam+'"]').addClass('soldout');
						 $('#variant-swatch-2-quickview .swatch-element[data-value="'+_tam+'"]').find('input').prop('disabled', true);				
					 }
				 })
				 $('#productQuickView .selector-wrapper .single-option-selector').eq(2).val(_available).trigger('change');
				 $('#variant-swatch-2-quickview .swatch-element[data-value="'+_available+'"] label').addClass('sd');
			 }
		 }else{

		 }
	 })

	 jQuery('#PageContainer').on('click','.input-product', function(e) {  
		 swatch_size = parseInt($('#product-select-watch').children().size());
		 e.preventDefault()
		 var $this = $(this);
		 var _available = '';
		 $this.parent().siblings().find('label').removeClass('sd');

		 $this.next().addClass('sd');

		 var name = $this.attr('name');
		 var value = $this.val();

		 console.log(value);

		 $('select[data-option='+name+']').val(value).trigger('change');

		 if(swatch_size == 2){
			 if(name.indexOf('1') != -1){
				 $('#variant-swatch-1 .swatch-element').find('input').prop('disabled', false);
				 $('#variant-swatch-2 .swatch-element').find('input').prop('disabled', false);
				 $('#variant-swatch-1 .swatch-element label').removeClass('sd');
				 $('#variant-swatch-1 .swatch-element').removeClass('soldout');
				 $('.selector-wrapper .single-option-selector').eq(1).find('option').each(function(){
					 var _tam = $(this).val();
					 $(this).parent().val(_tam).trigger('change');
					 if(check_variant){
						 if(_available == '' ){
							 _available = _tam;
						 }
					 }else{
						 $('#variant-swatch-1 .swatch-element[data-value="'+_tam+'"]').addClass('soldout');
						 $('#variant-swatch-1 .swatch-element[data-value="'+_tam+'"]').find('input').prop('disabled', true);
					 }
				 })
				 $('.selector-wrapper .single-option-selector').eq(1).val(_available).trigger('change');
				 $('#variant-swatch-1 .swatch-element[data-value="'+_available+'"] label').addClass('sd');
			 }
		 }
		 else if (swatch_size == 3){
			 var _count_op2 = $('#variant-swatch-1 .swatch-element').size();
			 var _count_op3 = $('#variant-swatch-2 .swatch-element').size();
			 if(name.indexOf('1') != -1){
				 $('#variant-swatch-1 .swatch-element').find('input').prop('disabled', false);
				 $('#variant-swatch-2 .swatch-element').find('input').prop('disabled', false);
				 $('#variant-swatch-1 .swatch-element label').removeClass('sd');
				 $('#variant-swatch-1 .swatch-element').removeClass('soldout');
				 $('#variant-swatch-2 .swatch-element label').removeClass('sd');
				 $('#variant-swatch-2 .swatch-element').removeClass('soldout');
				 var _avi_op1 = '';
				 var _avi_op2 = '';
				 $('#variant-swatch-1 .swatch-element').each(function(ind,value){
					 var _key = $(this).data('value');
					 var _unavi = 0;
					 $('.single-option-selector').eq(1).val(_key).change();
					 $('#variant-swatch-2 .swatch-element label').removeClass('sd');
					 $('#variant-swatch-2 .swatch-element').removeClass('soldout');
					 $('#variant-swatch-2 .swatch-element').find('input').prop('disabled', false);
					 $('#variant-swatch-2 .swatch-element').each(function(i,v){
						 var _val = $(this).data('value');
						 $('.single-option-selector').eq(2).val(_val).change();
						 if(check_variant == true){
							 if(_avi_op1 == ''){
								 _avi_op1 = _key;
							 }
							 if(_avi_op2 == ''){
								 _avi_op2 = _val;
							 }
							 //console.log(_avi_op1 + ' -- ' + _avi_op2)
						 }else{
							 _unavi += 1;
						 }
					 })
					 if(_unavi == _count_op3){
						 $('#variant-swatch-1 .swatch-element[data-value = "'+_key+'"]').addClass('soldout');
						 setTimeout(function(){
							 $('#variant-swatch-1 .swatch-element[data-value = "'+_key+'"] input').attr('disabled','disabled');
						 })
					 }
				 })
				 $('#variant-swatch-1 .swatch-element[data-value="'+_avi_op1+'"] input').click();
			 }
			 else if(name.indexOf('2') != -1){
				 $('#variant-swatch-2 .swatch-element label').removeClass('sd');
				 $('#variant-swatch-2 .swatch-element').removeClass('soldout');
				 $('.selector-wrapper .single-option-selector').eq(2).find('option').each(function(){
					 var _tam = $(this).val();
					 $(this).parent().val(_tam).trigger('change');
					 if(check_variant){
						 if(_available == '' ){
							 _available = _tam;
						 }
					 }else{
						 $('#variant-swatch-2 .swatch-element[data-value="'+_tam+'"]').addClass('soldout');
						 $('#variant-swatch-2 .swatch-element[data-value="'+_tam+'"]').find('input').prop('disabled', true);
					 }
				 })
				 $('.selector-wrapper .single-option-selector').eq(2).val(_available).trigger('change');
				 $('#variant-swatch-2 .swatch-element[data-value="'+_available+'"] label').addClass('sd');
			 }
		 }
		 else{
			 if(name.indexOf('1') != -1){

				 $('#variant-swatch-0 .swatch-element').find('input').prop('disabled', false);

				 $('#variant-swatch-0 .swatch-element label').removeClass('sd');

				 $('#variant-swatch-0 .swatch-element').removeClass('soldout');

				 $('.selector-wrapper .single-option-selector').eq(0).find('option').each(function(){

					 var _tam = $(this).val();

					 $(this).parent().val(_tam).trigger('change');

					 if(check_variant){
						 _available = _tam;
					 } else{
						 $('#variant-swatch-0 .swatch-element[data-value="'+_tam+'"]').addClass('soldout');
						 $('#variant-swatch-0 .swatch-element[data-value="'+_tam+'"]').find('input').prop('disabled', true);
					 }

				 })

				 $('.selector-wrapper .single-option-selector').eq(0).val(value).trigger('change');
				 $('#variant-swatch-0 .swatch-element[data-value="'+ value +'"] label').addClass('sd');
			 }
		 }
	 })
	 $(document).ready(function(){
		 var _chage = '';
		 $('.swatch-element[data-value="'+$('.selector-wrapper .single-option-selector').eq(0).val()+'"]').find('input').click();
		 $('.swatch-element[data-value="'+$('.selector-wrapper .single-option-selector').eq(1).val()+'"]').find('input').click();
		 if(swatch_size == 2){
			 var _avi_op1 = '';
			 var _avi_op2 = '';
			 var _count = $('#variant-swatch-1 .swatch-element').size();
			 $('#variant-swatch-0 .swatch-element').each(function(ind,value){
				 var _key = $(this).data('value');
				 var _unavi = 0;
				 $('.single-option-selector').eq(0).val(_key).change();
				 $('#variant-swatch-1 .swatch-element').each(function(i,v){
					 var _val = $(this).data('value');
					 $('.single-option-selector').eq(1).val(_val).change();
					 if(check_variant == true){
						 if(_avi_op1 == ''){
							 _avi_op1 = _key;
						 }
						 if(_avi_op2 == ''){
							 _avi_op2 = _val;
						 }
					 }else{
						 _unavi += 1;
					 }
				 })
				 if(_unavi == _count){
					 $('#variant-swatch-0 .swatch-element[data-value = "'+_key+'"]').addClass('soldout');
					 $('#variant-swatch-0 .swatch-element[data-value = "'+_key+'"]').find('input').attr('disabled','disabled');
				 }
			 })
			 $('#variant-swatch-1 .swatch-element[data-value = "'+_avi_op2+'"] input').click();
			 $('#variant-swatch-0 .swatch-element[data-value = "'+_avi_op1+'"] input').click();
		 }
		 else if(swatch_size == 3){
			 var _avi_op1 = '';
			 var _avi_op2 = '';
			 var _avi_op3 = '';
			 var _size_op2 = $('#variant-swatch-1 .swatch-element').size();
			 var _size_op3 = $('#variant-swatch-2 .swatch-element').size();

			 $('#variant-swatch-0 .swatch-element').each(function(ind,value){
				 var _key_va1 = $(this).data('value');
				 var _count_unavi = 0;
				 $('.single-option-selector').eq(0).val(_key_va1).change();
				 $('#variant-swatch-1 .swatch-element').each(function(i,v){
					 var _key_va2 = $(this).data('value');
					 var _unavi_2 = 0;
					 $('.single-option-selector').eq(1).val(_key_va2).change();
					 $('#variant-swatch-2 .swatch-element').each(function(j,z){
						 var _key_va3 = $(this).data('value');
						 $('.single-option-selector').eq(2).val(_key_va3).change();
						 if(check_variant == true){
							 if(_avi_op1 == ''){
								 _avi_op1 = _key_va1;
							 }
							 if(_avi_op2 == ''){
								 _avi_op2 = _key_va2;
							 }
							 if(_avi_op3 == ''){
								 _avi_op3 = _key_va3;
							 }
						 }else{
							 _unavi_2 += 1;
						 }
					 })
					 if(_unavi_2 == _size_op3){
						 _count_unavi += 1;
					 }
				 })
				 if(_size_op2 == _count_unavi){
					 $('#variant-swatch-0 .swatch-element[data-value = "'+_key_va1+'"]').addClass('soldout');
					 $('#variant-swatch-0 .swatch-element[data-value = "'+_key_va1+'"]').find('input').attr('disabled','disabled');
				 }
			 })
			 $('#variant-swatch-0 .swatch-element[data-value = "'+_avi_op1+'"] input').click();
		 }
	 })
 })