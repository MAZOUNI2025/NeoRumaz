# Souqify WooCommerce Theme

**Souqify** is a premium, translation-ready WooCommerce theme for hardware, tools and multi-niche eCommerce stores. It is designed for WordPress 6.x and WooCommerce 8.x+, with Arabic RTL and English LTR layouts, a responsive product-first interface, and a steel/red visual system.

## Installation

1. Copy the `souqify` directory to `wp-content/themes/`.
2. In WordPress, open **Appearance → Themes**, activate Souqify, then visit **Settings → Permalinks** and save once.
3. Install and activate WooCommerce. Create the Shop, Cart, Checkout and My Account pages from **WooCommerce → Settings → Advanced** if they are not already present.
4. Open **Appearance → Customize** to upload a logo, choose colors, enable or hide homepage sections, and set shop preferences.
5. Assign a menu to **Primary Navigation** and configure the footer widget areas under **Appearance → Widgets**.

## Recommended plugins

WooCommerce is required for products, cart, checkout, account pages, variable products, grouped products, galleries, ratings and sale pricing. For multilingual stores, use either **WPML** or **Polylang**. For wishlist functionality, **YITH WooCommerce Wishlist** is supported by the front-end button convention. A compatible WooCommerce multi-currency plugin should be used when converted prices and currency-specific checkout calculations are needed; the built-in selector persists the visitor's preference in localStorage and a cookie and exposes a `souqify:currencyChanged` browser event.

## Languages

Souqify uses the `souqify` text domain, `__()`/`esc_html__()` translation functions, and ships with `languages/souqify.pot`. Configure Arabic and English in WPML or Polylang. The theme detects the active WordPress locale for document direction and conditionally loads Cairo/Tajawal for RTL and Inter/DM Sans for LTR. The header language links also set the document direction immediately for a smoother switch; the multilingual plugin remains the source of truth for translated URLs and content.

## Currencies

The header selector includes DZD, SAR, AED, MAD, EGP, TND, QAR, USD, EUR and GBP. The selected code is stored as `souqify_currency` in localStorage and a cookie. A currency converter plugin is required to convert WooCommerce prices, taxes and payment amounts; a theme-level selector alone cannot safely alter those server-side financial calculations.

## WooCommerce compatibility

The theme supports WooCommerce product loops, variable and grouped products, product galleries with zoom/lightbox/slider support, sale and stock indicators, AJAX add to cart fragments, checkout fields, order review, account navigation, related products and WooCommerce's standard extension hooks. Template overrides intentionally retain WooCommerce actions so payment gateways and compatible extensions can continue to render their fields.

## Developer notes

All custom PHP functions use the `souqify_` prefix. Assets are split into `main.css`, `woocommerce.css`, `responsive.css` and `rtl.css`; JavaScript is split into core interactions, currency persistence and direction switching. Use a child theme for production customizations so future Souqify updates do not overwrite site-specific code.
