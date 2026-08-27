<?php
/** WooCommerce hooks, AJAX search and theme integrations. */

defined( 'ABSPATH' ) || exit;

function souqify_woocommerce_setup() { add_filter( 'woocommerce_enqueue_styles', '__return_empty_array' ); }
add_action( 'after_setup_theme', 'souqify_woocommerce_setup' );
function souqify_loop_columns() { return (int) souqify_get_option( 'shop_columns', 4 ); }
add_filter( 'loop_shop_columns', 'souqify_loop_columns' );
function souqify_products_per_page( $value ) { return (int) souqify_get_option( 'products_per_page', 12 ); }
add_filter( 'loop_shop_per_page', 'souqify_products_per_page', 20 );
function souqify_remove_wc_breadcrumb() { remove_action( 'woocommerce_before_main_content', 'woocommerce_breadcrumb', 20 ); }
add_action( 'init', 'souqify_remove_wc_breadcrumb' );
function souqify_cart_count_fragment( $fragments ) { ob_start(); ?><span class="cart-count"><?php echo function_exists( 'WC' ) && WC()->cart ? esc_html( WC()->cart->get_cart_contents_count() ) : '0'; ?></span><?php $fragments['.cart-count'] = ob_get_clean(); return $fragments; }
add_filter( 'woocommerce_add_to_cart_fragments', 'souqify_cart_count_fragment' );
function souqify_ajax_search() {
    check_ajax_referer( 'souqify_nonce', 'nonce' );
    $term = isset( $_GET['term'] ) ? sanitize_text_field( wp_unslash( $_GET['term'] ) ) : '';
    if ( strlen( $term ) < 2 ) { wp_send_json_success( array() ); }
    $query = new WP_Query( array( 'post_type' => 'product', 'post_status' => 'publish', 's' => $term, 'posts_per_page' => 6, 'no_found_rows' => true ) );
    $results = array();
    foreach ( $query->posts as $post ) { $product = wc_get_product( $post->ID ); if ( ! $product ) { continue; } $results[] = array( 'id' => $product->get_id(), 'name' => $product->get_name(), 'url' => $product->get_permalink(), 'image' => wp_get_attachment_image_url( $product->get_image_id(), 'thumbnail' ) ?: SOUQIFY_URI . '/assets/images/placeholder.png', 'price' => wp_strip_all_tags( $product->get_price_html() ) ); }
    wp_send_json_success( $results );
}
add_action( 'wp_ajax_souqify_search', 'souqify_ajax_search' );
add_action( 'wp_ajax_nopriv_souqify_search', 'souqify_ajax_search' );
function souqify_body_cart_class( $classes ) { if ( is_cart() ) { $classes[] = 'souqify-cart-page'; } return $classes; }
add_filter( 'body_class', 'souqify_body_cart_class' );
