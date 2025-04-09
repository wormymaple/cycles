shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen_tex : hint_screen_texture;
uniform sampler2D noise;
uniform float noise_intensity;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float dist = clamp(1.0 - distance(vec2(0.5, 0.5), UV) * 2.0, 0.0, 1.0);
	vec2 uv_shift = vec2(0.0, 1.0) * texture(noise, SCREEN_UV).x * noise_intensity * dist;
	vec4 screen_sample = texture(screen_tex, SCREEN_UV + uv_shift);
	COLOR = screen_sample;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
