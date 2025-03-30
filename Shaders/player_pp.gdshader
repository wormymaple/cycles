shader_type canvas_item;

uniform sampler2D screen_tex : hint_screen_texture;
uniform float warp_amount;

void vertex() {
	// Called for every vertex the material is visible on.
	VERTEX += vec2(warp_amount * VERTEX.y, 0.0);
}

void fragment() {
	vec2 uv = SCREEN_UV;
	COLOR = texture(screen_tex, uv);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
