<?php
/** Assets and front-end data. */

defined( 'ABSPATH' ) || exit;

function souqify_enqueue_assets() {
    $rtl = is_rtl();
    wp_enqueue_style( 'souqify-style', get_stylesheet_uri(), array(), SOUQIFY_VERSION );
    wp_enqueue_style( 'souqify-main', SOUQIFY_URI . '/assets/css/main.css', array( 'souqify-style' ), SOUQIFY_VERSION );
    wp_enqueue_style( 'souqify-woocommerce', SOUQIFY_URI . '/assets/css/woocommerce.css', array( 'souqify-main' ), SOUQIFY_VERSION );
    wp_enqueue_style( 'souqify-responsive', SOUQIFY_URI . '/assets/css/responsive.css', array( 'souqify-woocommerce' ), SOUQIFY_VERSION );
    if ( $rtl ) { wp_enqueue_style( 'souqify-rtl', SOUQIFY_URI . '/assets/css/rtl.css', array( 'souqify-responsive' ), SOUQIFY_VERSION ); }

    $font_family = $rtl ? 'Cairo:400,500,600,700,800|Tajawal:400,500,700' : 'Inter:400,500,600,700,800|DM+Sans:400,500,700';
    wp_enqueue_style( 'souqify-google-fonts', 'https://fonts.googleapis.com/css2?family=' . $font_family . '&display=swap', array(), null );
    wp_enqueue_script( 'souqify-main', SOUQIFY_URI . '/assets/js/main.js', array( 'jquery' ), SOUQIFY_VERSION, true );
    wp_enqueue_script( 'souqify-currency', SOUQIFY_URI . '/assets/js/currency.js', array(), SOUQIFY_VERSION, true );
    wp_enqueue_script( 'souqify-rtl-switcher', SOUQIFY_URI . '/assets/js/rtl-switcher.js', array(), SOUQIFY_VERSION, true );

    wp_localize_script( 'souqify-main', 'souqifyData', array(
        'ajaxUrl' => admin_url( 'admin-ajax.php' ),
        'nonce' => wp_create_nonce( 'souqify_nonce' ),
        'cartUrl' => function_exists( 'wc_get_cart_url' ) ? wc_get_cart_url() : '#',
        'checkoutUrl' => function_exists( 'wc_get_checkout_url' ) ? wc_get_checkout_url() : '#',
        'currency' => souqify_get_currency_data(),
        'i18n' => array( 'searching' => __( 'Searching…', 'souqify' ), 'noResults' => __( 'No products found.', 'souqify' ), 'added' => __( 'Added to cart.', 'souqify' ) ),
    ) );
}
add_action( 'wp_enqueue_scripts', 'souqify_enqueue_assets' );

function souqify_admin_assets( $hook ) {
    if ( 'customize.php' === $hook ) { wp_enqueue_style( 'souqify-admin', SOUQIFY_URI . '/assets/css/main.css', array(), SOUQIFY_VERSION ); }
}
add_action( 'admin_enqueue_scripts', 'souqify_admin_assets' );
