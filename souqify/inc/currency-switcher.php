<?php
/** Currency data and optional WooCommerce currency bridge. */

defined( 'ABSPATH' ) || exit;

function souqify_currencies() {
    return array(
        'DZD' => array( 'name' => __( 'Algerian Dinar', 'souqify' ), 'symbol' => 'د.ج', 'flag' => '🇩🇿' ),
        'SAR' => array( 'name' => __( 'Saudi Riyal', 'souqify' ), 'symbol' => 'ر.س', 'flag' => '🇸🇦' ),
        'AED' => array( 'name' => __( 'UAE Dirham', 'souqify' ), 'symbol' => 'د.إ', 'flag' => '🇦🇪' ),
        'MAD' => array( 'name' => __( 'Moroccan Dirham', 'souqify' ), 'symbol' => 'د.م', 'flag' => '🇲🇦' ),
        'EGP' => array( 'name' => __( 'Egyptian Pound', 'souqify' ), 'symbol' => 'ج.م', 'flag' => '🇪🇬' ),
        'TND' => array( 'name' => __( 'Tunisian Dinar', 'souqify' ), 'symbol' => 'د.ت', 'flag' => '🇹🇳' ),
        'QAR' => array( 'name' => __( 'Qatari Riyal', 'souqify' ), 'symbol' => 'ر.ق', 'flag' => '🇶🇦' ),
        'USD' => array( 'name' => __( 'US Dollar', 'souqify' ), 'symbol' => '$', 'flag' => '🇺🇸' ),
        'EUR' => array( 'name' => __( 'Euro', 'souqify' ), 'symbol' => '€', 'flag' => '🇪🇺' ),
        'GBP' => array( 'name' => __( 'British Pound', 'souqify' ), 'symbol' => '£', 'flag' => '🇬🇧' ),
    );
}
function souqify_get_currency_data() { return souqify_currencies(); }
function souqify_currency_switcher() {
    $currencies = souqify_currencies();
    $default = function_exists( 'get_woocommerce_currency' ) ? get_woocommerce_currency() : 'USD';
    if ( ! isset( $currencies[ $default ] ) ) { $default = 'USD'; }
    echo '<div class="currency-switcher" data-default-currency="' . esc_attr( $default ) . '"><button type="button" class="currency-toggle" aria-expanded="false" aria-controls="currency-options"><span class="currency-current-flag">' . esc_html( $currencies[ $default ]['flag'] ) . '</span><span class="currency-current-code">' . esc_html( $default ) . '</span><span class="currency-current-symbol">' . esc_html( $currencies[ $default ]['symbol'] ) . '</span>'; souqify_svg_icon( 'chevron' ); echo '</button><div id="currency-options" class="currency-options" hidden>'; foreach ( $currencies as $code => $currency ) { echo '<button type="button" class="currency-option" data-currency="' . esc_attr( $code ) . '" data-symbol="' . esc_attr( $currency['symbol'] ) . '"><span>' . esc_html( $currency['flag'] ) . '</span><span>' . esc_html( $currency['name'] ) . '</span><strong>' . esc_html( $currency['symbol'] ) . '</strong></button>'; } echo '</div></div>';
}
function souqify_ajax_currency( $currency ) { return sanitize_key( $currency ); }
