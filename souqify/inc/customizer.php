<?php
/** WordPress Customizer controls. */

defined( 'ABSPATH' ) || exit;

function souqify_customize_register( $wp_customize ) {
    $wp_customize->add_panel( 'souqify_general', array( 'title' => __( 'Souqify General', 'souqify' ), 'priority' => 20 ) );
    $wp_customize->add_section( 'souqify_branding', array( 'title' => __( 'Branding & Colors', 'souqify' ), 'panel' => 'souqify_general' ) );
    $wp_customize->add_setting( 'souqify_phone', array( 'default' => '+213 555 000 000', 'sanitize_callback' => 'sanitize_text_field' ) );
    $wp_customize->add_control( 'souqify_phone', array( 'label' => __( 'Phone number', 'souqify' ), 'section' => 'souqify_branding', 'type' => 'text' ) );
    $wp_customize->add_setting( 'souqify_primary_color', array( 'default' => '#E63329', 'sanitize_callback' => 'sanitize_hex_color' ) );
    $wp_customize->add_control( new WP_Customize_Color_Control( $wp_customize, 'souqify_primary_color', array( 'label' => __( 'Primary color', 'souqify' ), 'section' => 'souqify_branding' ) ) );
    $wp_customize->add_setting( 'souqify_accent_color', array( 'default' => '#F5A623', 'sanitize_callback' => 'sanitize_hex_color' ) );
    $wp_customize->add_control( new WP_Customize_Color_Control( $wp_customize, 'souqify_accent_color', array( 'label' => __( 'Accent color', 'souqify' ), 'section' => 'souqify_branding' ) ) );

    $wp_customize->add_section( 'souqify_header', array( 'title' => __( 'Header', 'souqify' ), 'panel' => 'souqify_general' ) );
    $wp_customize->add_setting( 'souqify_sticky_header', array( 'default' => true, 'sanitize_callback' => 'rest_sanitize_boolean' ) );
    $wp_customize->add_control( 'souqify_sticky_header', array( 'label' => __( 'Enable sticky header', 'souqify' ), 'section' => 'souqify_header', 'type' => 'checkbox' ) );
    $wp_customize->add_section( 'souqify_homepage', array( 'title' => __( 'Homepage Sections', 'souqify' ), 'panel' => 'souqify_general' ) );
    foreach ( array( 'hero' => 'Hero slider', 'promos' => 'Promo banners', 'categories' => 'Featured categories', 'deals' => 'Flash deals', 'trending' => 'Trending products', 'brands' => 'Brands strip', 'newsletter' => 'Newsletter' ) as $key => $label ) { $wp_customize->add_setting( 'souqify_show_' . $key, array( 'default' => true, 'sanitize_callback' => 'rest_sanitize_boolean' ) ); $wp_customize->add_control( 'souqify_show_' . $key, array( 'label' => __( 'Show ', 'souqify' ) . $label, 'section' => 'souqify_homepage', 'type' => 'checkbox' ) ); }
    $wp_customize->add_section( 'souqify_shop', array( 'title' => __( 'Shop', 'souqify' ), 'panel' => 'souqify_general' ) );
    $wp_customize->add_setting( 'souqify_products_per_page', array( 'default' => 12, 'sanitize_callback' => 'absint' ) );
    $wp_customize->add_control( 'souqify_products_per_page', array( 'label' => __( 'Products per page', 'souqify' ), 'section' => 'souqify_shop', 'type' => 'number', 'input_attrs' => array( 'min' => 4, 'max' => 48 ) ) );
    $wp_customize->add_setting( 'souqify_shop_columns', array( 'default' => 4, 'sanitize_callback' => 'absint' ) );
    $wp_customize->add_control( 'souqify_shop_columns', array( 'label' => __( 'Shop columns', 'souqify' ), 'section' => 'souqify_shop', 'type' => 'select', 'choices' => array( 2 => 2, 3 => 3, 4 => 4 ) ) );
    $wp_customize->add_section( 'souqify_footer', array( 'title' => __( 'Footer', 'souqify' ), 'panel' => 'souqify_general' ) );
    $wp_customize->add_setting( 'souqify_copyright', array( 'default' => '© ' . gmdate( 'Y' ) . ' Souqify. All rights reserved.', 'sanitize_callback' => 'wp_kses_post' ) );
    $wp_customize->add_control( 'souqify_copyright', array( 'label' => __( 'Copyright text', 'souqify' ), 'section' => 'souqify_footer', 'type' => 'textarea' ) );
}
add_action( 'customize_register', 'souqify_customize_register' );

function souqify_customizer_css() { echo '<style>:root{--primary:' . esc_attr( souqify_get_option( 'primary_color', '#E63329' ) ) . ';--accent:' . esc_attr( souqify_get_option( 'accent_color', '#F5A623' ) ) . ';}</style>'; }
add_action( 'wp_head', 'souqify_customizer_css' );
