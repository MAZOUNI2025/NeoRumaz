<?php
defined( 'ABSPATH' ) || exit;

global $product;
if ( ! $product || ! is_a( $product, 'WC_Product' ) ) {
    $product = wc_get_product( get_the_ID() );
}
if ( ! $product ) {
    return;
}
get_template_part( 'template-parts/product/product-card' );
