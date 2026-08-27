<?php
/** Theme setup and global registrations. */

defined( 'ABSPATH' ) || exit;

function souqify_setup() {
    load_theme_textdomain( 'souqify', SOUQIFY_DIR . '/languages' );
    add_theme_support( 'automatic-feed-links' );
    add_theme_support( 'title-tag' );
    add_theme_support( 'post-thumbnails' );
    add_theme_support( 'custom-logo', array( 'height' => 80, 'width' => 240, 'flex-height' => true, 'flex-width' => true ) );
    add_theme_support( 'html5', array( 'search-form', 'comment-form', 'comment-list', 'gallery', 'caption', 'style', 'script' ) );
    add_theme_support( 'custom-background', array( 'default-color' => 'f4f6f9' ) );
    add_theme_support( 'responsive-embeds' );
    add_theme_support( 'woocommerce', array( 'thumbnail_image_width' => 720, 'single_image_width' => 900, 'product_grid' => array( 'default_rows' => 4, 'min_rows' => 1, 'max_rows' => 8, 'default_columns' => 4, 'min_columns' => 2, 'max_columns' => 4 ) ) );
    add_theme_support( 'wc-product-gallery-zoom' );
    add_theme_support( 'wc-product-gallery-lightbox' );
    add_theme_support( 'wc-product-gallery-slider' );

    register_nav_menus( array(
        'primary' => __( 'Primary Navigation', 'souqify' ),
        'footer'  => __( 'Footer Navigation', 'souqify' ),
        'mobile'  => __( 'Mobile Navigation', 'souqify' ),
    ) );

    add_image_size( 'souqify-product-card', 720, 720, true );
    add_image_size( 'souqify-hero', 1600, 720, true );
    add_image_size( 'souqify-promo', 1000, 640, true );
}
add_action( 'after_setup_theme', 'souqify_setup' );

function souqify_content_width() { $GLOBALS['content_width'] = apply_filters( 'souqify_content_width', 1200 ); }
add_action( 'after_setup_theme', 'souqify_content_width', 0 );

function souqify_widgets_init() {
    $areas = array(
        'sidebar-1' => __( 'Shop Sidebar', 'souqify' ),
        'footer-1' => __( 'Footer Column 1', 'souqify' ),
        'footer-2' => __( 'Footer Column 2', 'souqify' ),
        'footer-3' => __( 'Footer Column 3', 'souqify' ),
        'footer-4' => __( 'Footer Column 4', 'souqify' ),
    );
    foreach ( $areas as $id => $name ) {
        register_sidebar( array( 'name' => $name, 'id' => $id, 'description' => $name, 'before_widget' => '<section id="%1$s" class="widget %2$s">', 'after_widget' => '</section>', 'before_title' => '<h3 class="widget-title">', 'after_title' => '</h3>' ) );
    }
}
add_action( 'widgets_init', 'souqify_widgets_init' );

function souqify_body_classes( $classes ) {
    $classes[] = 'souqify-theme';
    if ( class_exists( 'WooCommerce' ) ) { $classes[] = 'woocommerce-active'; }
    if ( is_front_page() ) { $classes[] = 'souqify-front-page'; }
    return $classes;
}
add_filter( 'body_class', 'souqify_body_classes' );

function souqify_language_attributes( $output ) {
    $locale = function_exists( 'determine_locale' ) ? determine_locale() : get_locale();
    $rtl = preg_match( '/^(ar|fa|he|ur)(_|-)/i', $locale );
    return 'dir="' . ( $rtl ? 'rtl' : 'ltr' ) . '" lang="' . esc_attr( str_replace( '_', '-', $locale ) ) . '"';
}
add_filter( 'language_attributes', 'souqify_language_attributes' );
