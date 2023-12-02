(function($) {

    if (typeof window.CustomEvent !== "function") {
        function CustomEvent( event, params ) {
            params = params || { bubbles: false, cancelable: false, detail: undefined };
            var evt = document.createEvent( 'CustomEvent' );
            evt.initCustomEvent( event, params.bubbles, params.cancelable, params.detail );
            return evt;
        }
        CustomEvent.prototype = window.Event.prototype;
        window.CustomEvent = CustomEvent;
    }

    function initTheGemFullpage() {
        window.gemSettings.fullpageEnabled = true;

        let fullpageId = '#thegem-fullpage',
            sectionSelector = '.scroller-block',
            anchorAttrName = 'data-anchor',
            $body = $('body'),
            $fullpage = $(fullpageId),
            isDisabledDots = $body.hasClass('thegem-fp-disabled-dots'),
            isDisabledTooltips = $body.hasClass('thegem-fp-disabled-tooltips'),
            isEnableAnchor = $(sectionSelector+'['+anchorAttrName+']').length !== 0,
            isFixedBackground = $body.hasClass('thegem-fp-fixed-background'),
            isDisabledMobile = $body.hasClass('thegem-fp-disabled-mobile'),
            isEnableContinuous = $body.hasClass('thegem-fp-enable-continuous'),
            isEnabledParallax = $body.hasClass('thegem-fp-parallax'),
            menuSelector = '#primary-menu',
            anchors = [];

        let options = {
            sectionSelector: sectionSelector,
            verticalCentered: false,
            navigation: !isDisabledDots,
            autoScrolling: true,
            navigationTooltips: isDisabledTooltips ? [''] : [],
            anchors: anchors,
            lockAnchors: !isEnableAnchor,
            css3: !isFixedBackground,
            responsiveWidth: isDisabledMobile ? 768 : 0,
            continuousVertical: isEnableContinuous,
            licenseKey: ''
        };

        if (isEnabledParallax) {
            options.scrollingSpeed = 1000;
        }

        if (isEnableAnchor) {
            options.menu = menuSelector;

            let anchorItems = [];
            $fullpage.find(sectionSelector).each(function(idx, item) {
                let anchor = $(item).attr(anchorAttrName);
                if (anchor===undefined || anchor===$(item).attr('id')) {
                    $(item).attr(anchorAttrName, 'section-'+(idx+1));
                }
                anchorItems.push($(item).attr(anchorAttrName));
            });

            $('li', menuSelector).each(function(idx, item) {
                let link = $('a', item);
                if (link.length) {
                    let anchor = link.attr('href').replace('#','');
                    if (anchorItems.indexOf(anchor)!==-1) {
                        $(item).attr('data-menuanchor', anchor);
                    }
                }
            });
        }

        options.onLeave = function(origin, destination, direction) {
            if (isEnableAnchor) {
                activateMenuElement(menuSelector, destination);
            }

            if (isEnabledParallax && direction) {
                if (direction === 'up') {
                    $(origin.item).addClass('fp-prev-down');
                    $(destination.item).addClass('fp-next-down');
                }
                if (direction === 'down') {
                    $(origin.item).addClass('fp-prev-up');
                    $(destination.item).addClass('fp-next-up');
                }
            }

            sendCustomEvent();
        };

        options.afterLoad = function(origin, destination, direction) {
            if (destination.index === 0 && !$(destination.item).hasClass('fp-section-initialized')) {
                $(destination.item).addClass('fp-section-initialized');
            }

            if (isEnabledParallax && direction) {
                $(sectionSelector).removeClass('fp-prev-down fp-next-down fp-prev-up fp-next-up');
            }

            if (isEnableAnchor) {
                activateMenuElement(menuSelector, destination);
                sendCustomEvent();
            }

            if (destination.index > 0 && !$(destination.item).hasClass('fp-section-initialized')) {
                $(destination.item).addClass('fp-section-initialized');
            if(window.vc_waypoints)
                window.vc_waypoints();

                $('.lazy-loading-item', destination.item).each(function (index, item) {
                    $.lazyLoading();
                });

                $('.vc_chart:not(".vc_chart-initialized")', destination.item).each(function (index, item) {
                    $(item).addClass('vc_chart-initialized');

                    if ($(item).hasClass('vc_round-chart')) {
                        $(item).vcRoundChart();
                    }

                    if ($(item).hasClass('vc_line-chart')) {
                        $(item).vcLineChart();
                    }
                });

                $('.vc_pie_chart:not(".vc_pie_chart-initialized")', destination.item).each(function (index, item) {
                    $(item).addClass('vc_pie_chart-initialized');
                    $(item).vcChat();
                });

                $('.vc_progress_bar:not(".vc_progress_bar-initialized")', destination.item).each(function (index, item) {
                    $(item).addClass('vc_progress_bar-initialized');
                    window.vc_progress_bar();
                });

                $('.portfolio, .news-grid', destination.item).each(function(index, item) {
                    var $portfolio = $(item);
                    $portfolio.itemsAnimations('instance').reinitItems($('.portfolio-set .portfolio-item', $portfolio));
                    $('.portfolio-set', $portfolio).isotope();
                });
                
                $('.gem-gallery-grid', destination.item).each(function(index, item) {
                    var $galleryGrid = $(item);
                    var $items = $('.gallery-set .gallery-item', $galleryGrid)
                    $galleryGrid.itemsAnimations('instance').reinitItems($items);
                    $('.gallery-set', $galleryGrid).isotope();

                    setTimeout(function () {
                        $galleryGrid.itemsAnimations('instance').show($items);
                    }, 300);
                });
            }
        };

        options.afterResponsive = function (isResponsive) {
            window.gemSettings.fullpageEnabled = isResponsive && !isDisabledMobile;
        };

        new fullpage(fullpageId, options);
    }

    function sendCustomEvent() {
        document.dispatchEvent(new window.CustomEvent('fullpage-updated'));
    }
    
    function activateMenuElement(menuSelector, destination){
        $('li', menuSelector).removeClass('menu-item-active');
        $(menuSelector).find('[data-menuanchor="'+destination.anchor+'"]', 'li').addClass('menu-item-active');
    }
    
    initTheGemFullpage();

})(window.jQuery);