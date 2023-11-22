(function ($) {

	'use strict';

	const $body = $('body');
	const $holder = $('#td-overlay-holder');
	const $header = $('#site-header');
	const $preview = $('#td-overlay-preview');
	const $iframe = $('#td-overlay-iframe');

	const setCookie = (name, value, options) => {
		options = options || {};

		let expires = options.expires;

		if (typeof expires == "number" && expires) {
			let d = new Date();
			d.setTime(d.getTime() + expires * 1000);
			expires = options.expires = d;
		}

		if (expires && expires.toUTCString) {
			options.expires = expires.toUTCString();
		}

		value = encodeURIComponent(value);

		let updatedCookie = name + "=" + value;

		for (let propName in options) {
			updatedCookie += "; " + propName;
			let propValue = options[propName];
			if (propValue !== true) {
				updatedCookie += "=" + propValue;
			}
		}

		document.cookie = updatedCookie;
	}

	const getCookie = (name) => {
		let matches = document.cookie.match(new RegExp(
			"(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
		));
		return matches ? decodeURIComponent(matches[1]) : undefined;
	}

	const setBodyLocked = () => {
		$body.addClass('td-overlay-scroll-locked');

		// Set Iframe
		$preview.addClass('preview-frame-loading');
		$iframe.attr("src", $iframe.data('src'));

		if( $iframe.length && $iframe[0].complete ) {
			$preview.removeClass('preview-frame-loading');
		} else {
			$iframe.on('load error', function () {
				$preview.removeClass('preview-frame-loading');
			})
		}

		// Huck for iframe show
		setTimeout(() => $preview.removeClass('preview-frame-loading'), 2000);
	}

	const unsetBodyLocked = () => {
		$body.removeClass('td-overlay-scroll-locked');

		// Unset Iframe
		$iframe.removeAttr("src");
	}

	const scripts = {
		init: () => {
			scripts.initButton();
			scripts.actionButton();
		},

		initButton: () => {
			if (!getCookie('thegem-demos-tooltip')) {
				$('.td-overlay-button-demos__tooltip', $holder).fadeIn();

				$holder.on('click', '.td-overlay-button-demos', function() {
					setCookie('thegem-demos-tooltip', 1, { path: '/', SameSite: 'None', Secure: true });
					$('.td-overlay-button-demos__tooltip', $(this)).fadeOut();
				});
			}
		},

		actionButton: () => {
			$holder.on('click', '.td-overlay-button-demos', function() {
				setBodyLocked();

				$('.td-overlay-button-demos__label', $holder).addClass('td-preview-open');
				$('.td-overlay-button-shop__label', $holder).addClass('td-preview-open');
				$preview.addClass('td-preview-open');
			});

			$('.td-demos-overlay-open').on('click', 'a', function(e) {
				e.preventDefault();
				setBodyLocked();

				$('.td-overlay-button-demos__label', $holder).addClass('td-preview-open');
				$('.td-overlay-button-shop__label', $holder).addClass('td-preview-open');
				$preview.addClass('td-preview-open');

				if (typeof(gtag) !== 'function') {
					return;
				}
				gtag('event', 'click', {'event_category': 'DEMOS_BUTTON_NEW', 'event_label': 'Menu Demos Button New'});
			});

			$preview.on('click', '.td-overlay-preview-close', function() {
				unsetBodyLocked();

				$('.td-overlay-button-demos__label', $holder).removeClass('td-preview-open');
				$('.td-overlay-button-shop__label', $holder).removeClass('td-preview-open');
				$preview.removeClass('td-preview-open');
			});

			$holder.on('click', '.td-ga-event-button', function(e) {
				if (typeof(gtag) !== 'function') {
					return;
				}

				let id = $(this).data('id');

				switch (id) {
					case 'td-overlay-button-demos':
						gtag('event', 'click', {'event_category': '400_BUTTON_NEW', 'event_label': '400+ Demos Button New'});
						break;
					case 'td-overlay-button-shop':
						gtag('event', 'click', {'event_category': 'BUY_NOW_NEW', 'event_label': 'Buy Now Button New'});
						break;
				}
			});

			if (typeof window.gemDemosOverlayClicked !== 'undefined' && window.gemDemosOverlayClicked) {
				$('.td-overlay-button-event').trigger('click');
				window.gemDemosOverlayClicked = null;
			}

			if (typeof window.gemDemosOverlayMenuClicked !== 'undefined' && window.gemDemosOverlayMenuClicked) {
				$('.td-demos-overlay-open > a').trigger('click');
				window.gemDemosOverlayMenuClicked = null;
			}
		}
	}

	// Run the function
	$(function () {
		scripts.init();
	});
})(jQuery);