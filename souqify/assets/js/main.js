(function($){'use strict';
function debounce(fn, wait){var t;return function(){var ctx=this,args=arguments;clearTimeout(t);t=setTimeout(function(){fn.apply(ctx,args);},wait);};}
$(function(){
  var header=$('.site-header');
  if(header.data('sticky-header')==='1'){ $(window).on('scroll',function(){header.toggleClass('is-sticky',window.scrollY>80);$('.back-to-top').toggleClass('is-visible',window.scrollY>400);}); }
  $('.mobile-menu-toggle').on('click',function(){$('.primary-nav').addClass('is-open');$('body').addClass('menu-open');});
  $('.mobile-menu-close').on('click',function(){$('.primary-nav').removeClass('is-open');$('body').removeClass('menu-open');});
  $('.header-search form>.icon').on('click',function(e){if(window.innerWidth<768){e.preventDefault();$('.header-search').toggleClass('is-open').find('input[type=search]').trigger('focus');}});
  $('.back-to-top').on('click',function(){$('html,body').animate({scrollTop:0},300);});
  $('.currency-toggle').on('click',function(){var b=$(this),o=b.attr('aria-expanded')==='true';b.attr('aria-expanded',String(!o));b.next().prop('hidden',o);});
  $(document).on('click',function(e){if(!$(e.target).closest('.currency-switcher').length){$('.currency-toggle').attr('aria-expanded','false');$('.currency-options').prop('hidden',true);}});
  var searchBox=$('#header-search-input'),results=$('.live-search-results');
  searchBox.on('input',debounce(function(){var term=$(this).val();if(term.length<2){results.prop('hidden',true).empty();return;}results.removeAttr('hidden').html('<div class="search-loading">'+souqifyData.i18n.searching+'</div>');$.get(souqifyData.ajaxUrl,{action:'souqify_search',nonce:souqifyData.nonce,term:term}).done(function(res){if(!res.success||!res.data.length){results.html('<div class="search-loading">'+souqifyData.i18n.noResults+'</div>');return;}results.html(res.data.map(function(item){return '<a class="live-search-item" href="'+item.url+'"><img src="'+item.image+'" alt=""><span><strong>'+item.name+'</strong><span>'+item.price+'</span></span></a>';}).join(''));});},250));
  $('.hero-slider').each(function(){var slider=$(this),slides=slider.find('.hero-slide'),dots=slider.find('.hero-dots button'),index=0,timer;function show(i){index=(i+slides.length)%slides.length;slides.removeClass('is-active').eq(index).addClass('is-active');dots.removeClass('is-active').eq(index).addClass('is-active');}function start(){clearInterval(timer);timer=setInterval(function(){show(index+1);},parseInt(slider.data('autoplay'),10)||5000);}slider.find('.hero-next').on('click',function(){show(index+1);start();});slider.find('.hero-prev').on('click',function(){show(index-1);start();});dots.on('click',function(){show($(this).index());start();});start();});
  $('.countdown').each(function(){var el=$(this),parts=(el.data('countdown')||'24:00:00').split(':'),seconds=(+parts[0]*3600)+(+parts[1]*60)+(+parts[2]);setInterval(function(){seconds=Math.max(0,seconds-1);var h=String(Math.floor(seconds/3600)).padStart(2,'0'),m=String(Math.floor(seconds%3600/60)).padStart(2,'0'),s=String(seconds%60).padStart(2,'0');el.find('.countdown-time').text(h+':'+m+':'+s);},1000);});
  $('.filter-toggle').on('click',function(){$('.shop-sidebar').toggleClass('is-visible');});
  $('.view-toggle').on('click',function(){var view=$(this).data('view');$('.view-toggle').removeClass('is-active');$(this).addClass('is-active');$('.products-grid').toggleClass('list-view',view==='list');});
  $('.quick-view-button').on('click',function(){var id=$(this).data('product-id');$('.quick-view-modal').removeAttr('hidden');$('.quick-view-content').html('<p>'+souqifyData.i18n.searching+'</p>');$.get(souqifyData.ajaxUrl,{action:'souqify_search',nonce:souqifyData.nonce,term:''}).always(function(){$('.quick-view-content').html('<h2 id="quick-view-title">'+$('article.post-'+id+' .product-title').text()+'</h2><p>'+souqifyData.i18n.noResults+'</p>');});});
  $(document).on('click','.quick-view-close,.quick-view-backdrop',function(){$('.quick-view-modal').attr('hidden',true);});
  $(document.body).on('added_to_cart',function(){$('.cart-trigger').addClass('cart-bump');setTimeout(function(){$('.cart-trigger').removeClass('cart-bump');},500);});
});
})(jQuery);
