<?php
/**
 * Souqify theme bootstrap.
 *
 * @package Souqify
 */

defined( 'ABSPATH' ) || exit;

define( 'SOUQIFY_VERSION', '1.0.0' );
define( 'SOUQIFY_DIR', get_template_directory() );
define( 'SOUQIFY_URI', get_template_directory_uri() );

require_once SOUQIFY_DIR . '/inc/theme-setup.php';
require_once SOUQIFY_DIR . '/inc/enqueue.php';
require_once SOUQIFY_DIR . '/inc/customizer.php';
require_once SOUQIFY_DIR . '/inc/widgets.php';
require_once SOUQIFY_DIR . '/inc/helpers.php';
require_once SOUQIFY_DIR . '/inc/currency-switcher.php';
require_once SOUQIFY_DIR . '/inc/woocommerce-hooks.php';
