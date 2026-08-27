<?php
/** Small reusable template helpers. */

defined( 'ABSPATH' ) || exit;

function souqify_get_option( $key, $default = '' ) { return get_theme_mod( 'souqify_' . $key, $default ); }
function souqify_logo() {
    if ( has_custom_logo() ) { the_custom_logo(); return; }
    echo '<a class="site-logo" href="' . esc_url( home_url( '/' ) ) . '" rel="home"><span class="logo-mark">S</span><span class="logo-word">Souqify</span></a>';
}
function souqify_product_price( $product = null ) {
    if ( ! $product && function_exists( 'wc_get_product' ) ) { global $product; }
    return $product && is_object( $product ) ? $product->get_price_html() : '';
}
function souqify_product_badges( $product ) {
    if ( ! $product ) { return; }
    echo '<div class="product-badges">';
    if ( ! $product->is_in_stock() ) { echo '<span class="badge badge-out">' . esc_html__( 'Out of stock', 'souqify' ) . '</span>'; }
    elseif ( $product->is_on_sale() ) { echo '<span class="badge badge-sale">' . esc_html__( 'Sale', 'souqify' ) . '</span>'; }
    elseif ( (int) $product->get_id() > ( time() - 30 * DAY_IN_SECONDS ) ) { echo '<span class="badge badge-new">' . esc_html__( 'New', 'souqify' ) . '</span>'; }
    echo '</div>';
}
function souqify_stock_indicator( $product ) {
    if ( ! $product || ! $product->managing_stock() || ! $product->is_in_stock() ) { return; }
    $stock = (int) $product->get_stock_quantity();
    if ( $stock > 0 && $stock <= 10 ) { echo '<div class="stock-indicator"><span>' . sprintf( esc_html__( 'Only %s left in stock', 'souqify' ), esc_html( $stock ) ) . '</span><span class="stock-bar"><i style="width:' . esc_attr( min( 100, $stock * 10 ) ) . '%"></i></span></div>'; }
}
function souqify_pagination() { the_posts_pagination( array( 'mid_size' => 2, 'prev_text' => '‹ ' . __( 'Previous', 'souqify' ), 'next_text' => __( 'Next', 'souqify' ) . ' ›' ) ); }
function souqify_breadcrumbs() {
    if ( function_exists( 'woocommerce_breadcrumb' ) ) { woocommerce_breadcrumb( array( 'delimiter' => '<span class="breadcrumb-separator">/</span>' ) ); return; }
    echo '<nav class="breadcrumbs" aria-label="' . esc_attr__( 'Breadcrumbs', 'souqify' ) . '"><a href="' . esc_url( home_url( '/' ) ) . '">' . esc_html__( 'Home', 'souqify' ) . '</a><span>/</span><span>' . esc_html( get_the_title() ) . '</span></nav>';
}
function souqify_svg_icon( $name, $label = '' ) {
    $icons = array( 'search' => '<circle cx="11" cy="11" r="7"></circle><path d="m20 20-4-4"></path>', 'cart' => '<circle cx="9" cy="20" r="1"></circle><circle cx="18" cy="20" r="1"></circle><path d="M2 3h3l2.4 12.1a2 2 0 0 0 2 1.6h8.7a2 2 0 0 0 2-1.6L22 7H6"></path>', 'user' => '<circle cx="12" cy="8" r="4"></circle><path d="M4 21a8 8 0 0 1 16 0"></path>', 'heart' => '<path d="M20.8 8.7c0 5.5-8.8 10.3-8.8 10.3S3.2 14.2 3.2 8.7A4.7 4.7 0 0 1 12 6.1a4.7 4.7 0 0 1 8.8 2.6Z"></path>', 'menu' => '<path d="M3 6h18M3 12h18M3 18h18"></path>', 'close' => '<path d="m6 6 12 12M18 6 6 18"></path>', 'arrow' => '<path d="M5 12h14M13 6l6 6-6 6"></path>', 'chevron' => '<path d="m6 9 6 6 6-6"></path>', 'phone' => '<path d="M22 16.9v3a2 2 0 0 1-2.2 2 19.8 19.8 0 0 1-8.6-3.1 19.5 19.5 0 0 1-6-6A19.8 19.8 0 0 1 2.1 4.2 2 2 0 0 1 4.1 2h3a2 2 0 0 1 2 1.7c.1 1 .4 2 .7 2.8a2 2 0 0 1-.5 2.1L8 9.9a16 16 0 0 0 6 6l1.3-1.3a2 2 0 0 1 2.1-.5c.9.3 1.8.6 2.8.7a2 2 0 0 1 1.8 2.1Z"></path>' );
    if ( ! isset( $icons[ $name ] ) ) { return; }
    echo '<svg class="icon icon-' . esc_attr( $name ) . '" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">' . $icons[ $name ] . '</svg>';
    if ( $label ) { echo '<span class="screen-reader-text">' . esc_html( $label ) . '</span>'; }
}
function souqify_get_image_url( $id = 0, $size = 'large' ) { return $id ? wp_get_attachment_image_url( $id, $size ) : SOUQIFY_URI . '/assets/images/placeholder.png'; }
