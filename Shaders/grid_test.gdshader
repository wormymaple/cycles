shader_type canvas_item;
uniform float warp_amount;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec2 newUVs = vec2(UV.x + UV.y * warp_amount * (0.5 - UV.x), UV.y);
	COLOR = texture(TEXTURE, newUVs);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
