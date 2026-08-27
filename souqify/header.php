<?php
defined( 'ABSPATH' ) || exit;
?><!doctype html>
<html <?php language_attributes(); ?>>
<head>
<meta charset="<?php bloginfo( 'charset' ); ?>">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta name="theme-color" content="#E63329">
<?php wp_head(); ?>
</head>
<body <?php body_class(); ?>>
<?php wp_body_open(); ?>
<a class="skip-link screen-reader-text" href="#content"><?php esc_html_e( 'Skip to content', 'souqify' ); ?></a>
<div id="page" class="site">
<?php get_template_part( 'template-parts/header/top-bar' ); ?>
<header class="site-header" data-sticky-header="<?php echo souqify_get_option( 'sticky_header', true ) ? '1' : '0'; ?>">
<?php get_template_part( 'template-parts/header/main-header' ); ?>
<?php get_template_part( 'template-parts/header/nav-menu' ); ?>
</header>
<main id="content" class="site-content">
