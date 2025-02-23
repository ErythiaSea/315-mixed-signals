shader_type canvas_item;
render_mode blend_add;

uniform float circle_r = 0.f;
uniform float brightness = 1.8f;

uniform float fall_off_scale = 2.0f;
uniform float b_offset = 0.1f;


void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 uv = UV - vec2(.5);
	vec2 closestCirclePoint;
	closestCirclePoint = uv;
	closestCirclePoint.x = clamp(uv.x, -circle_r, circle_r);
	closestCirclePoint.y = clamp(uv.y, -circle_r, circle_r);
	
	vec2 cuv = uv - closestCirclePoint;
	float d2c = length(cuv);
	
	COLOR.a = -log(d2c*fall_off_scale + b_offset) * brightness;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
