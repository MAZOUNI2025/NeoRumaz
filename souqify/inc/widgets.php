<?php
/** Custom widgets used by the theme. */

defined( 'ABSPATH' ) || exit;

class Souqify_Contact_Widget extends WP_Widget {
    public function __construct() { parent::__construct( 'souqify_contact', __( 'Souqify Contact', 'souqify' ), array( 'description' => __( 'Contact information widget.', 'souqify' ) ) ); }
    public function widget( $args, $instance ) { echo $args['before_widget']; echo $args['before_title'] . esc_html( $instance['title'] ?? __( 'Contact us', 'souqify' ) ) . $args['after_title']; echo '<div class="widget-contact"><p>' . esc_html( $instance['phone'] ?? '+213 555 000 000' ) . '</p><p>' . esc_html( $instance['email'] ?? 'hello@example.com' ) . '</p></div>'; echo $args['after_widget']; }
    public function form( $instance ) { foreach ( array( 'title' => __( 'Title', 'souqify' ), 'phone' => __( 'Phone', 'souqify' ), 'email' => __( 'Email', 'souqify' ) ) as $key => $label ) { echo '<p><label>' . esc_html( $label ) . '</label><input class="widefat" name="' . esc_attr( $this->get_field_name( $key ) ) . '" value="' . esc_attr( $instance[ $key ] ?? '' ) . '"></p>'; } }
    public function update( $new, $old ) { return array_map( 'sanitize_text_field', $new ); }
}
function souqify_register_widgets() { register_widget( 'Souqify_Contact_Widget' ); }
add_action( 'widgets_init', 'souqify_register_widgets' );
