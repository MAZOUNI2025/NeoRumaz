<?php
defined( 'ABSPATH' ) || exit;

global $product;
if ( ! $product || ! is_a( $product, 'WC_Product' ) ) {
    return;
}
?>
<article id="product-<?php the_ID(); ?>" <?php wc_product_class( 'souqify-single-product', $product ); ?>>
    <div class="single-product-layout">
        <div class="single-product-gallery">
            <?php do_action( 'woocommerce_before_single_product_summary' ); ?>
        </div>
        <div class="single-product-summary summary entry-summary">
            <div class="product-meta-row">
                <?php if ( $product->get_sku() ) : ?><span><?php esc_html_e( 'SKU:', 'souqify' ); ?> <?php echo esc_html( $product->get_sku() ); ?></span><?php endif; ?>
                <?php if ( $product->is_in_stock() ) : ?><span class="availability in-stock"><?php esc_html_e( 'In stock', 'souqify' ); ?></span><?php else : ?><span class="availability out-of-stock"><?php esc_html_e( 'Out of stock', 'souqify' ); ?></span><?php endif; ?>
            </div>
            <?php do_action( 'woocommerce_single_product_summary' ); ?>
            <div class="trust-badges"><span>✓ <?php esc_html_e( 'Free shipping', 'souqify' ); ?></span><span>✓ <?php esc_html_e( 'Warranty', 'souqify' ); ?></span><span>✓ <?php esc_html_e( 'Easy returns', 'souqify' ); ?></span><span>✓ <?php esc_html_e( 'Secure pay', 'souqify' ); ?></span></div>
        </div>
    </div>
    <div class="single-product-tabs"><?php do_action( 'woocommerce_after_single_product_summary' ); ?></div>
</article>
