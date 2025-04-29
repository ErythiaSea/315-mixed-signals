// from the free open source Godot Shaders library
// https://github.com/gdquest-demos/godot-shaders/blob/main/godot/Shaders/outline2D_outer.gdshader

shader_type canvas_item;

uniform vec3 line_color : source_color = vec3(1.0);
uniform float line_alpha = 1.0;
uniform float line_thickness : hint_range(0, 10) = 1.0;

const vec2 OFFSETS[8] = {
	vec2(-1, -1), vec2(-1, 0), vec2(-1, 1), vec2(0, -1), vec2(0, 1),
	vec2(1, -1), vec2(1, 0), vec2(1, 1)
};

void fragment() {
	vec2 size = TEXTURE_PIXEL_SIZE * line_thickness;
	float outline = 0.0;

	for (int i = 0; i < OFFSETS.length(); i++) {
		outline += texture(TEXTURE, UV + size * OFFSETS[i]).a;
	}
	outline = min(outline, 1.0);

	vec4 color = texture(TEXTURE, UV);
	COLOR = mix(color, vec4(line_color, line_alpha), outline - color.a);
}