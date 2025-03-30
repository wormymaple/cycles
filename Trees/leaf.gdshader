shader_type canvas_item;
render_mode world_vertex_coords;

uniform float wave_speed;
uniform float wave_intensity;
uniform sampler2D season_gradient;
uniform float season_value;
uniform sampler2D noise : repeat_enable;
uniform vec2 worldPos;

void vertex() {
	float noise_x = texture(noise, vec2((TIME + VERTEX.y + VERTEX.x + worldPos.x + worldPos.y) * wave_speed, 0.)).x - 0.5;
	float noise_y = texture(noise, vec2(0., (TIME + VERTEX.y + VERTEX.x + worldPos.x + worldPos.y) * wave_speed)).x - 0.5;
	VERTEX += vec2(noise_x, noise_y) * wave_intensity;
}

void fragment() {
	COLOR *= texture(season_gradient, vec2(season_value, 0.0));
}
