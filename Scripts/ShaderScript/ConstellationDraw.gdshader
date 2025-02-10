shader_type canvas_item;

float circle(vec2 uv,float radius,float blur){
	float d = length(uv);
	float c = 0.0;
	
	//if(d < r) c = 1.0f;
	c = smoothstep(radius,radius - blur,d);
	
	return c;
	
}
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
// Called for every pixel the material is visible on.
    vec2 uv = UV - 0.5;
	//multiply by aspect ratio
	uv.x *= 1.706666667;
	float c1 = circle(uv,0.4,0.01);
	
	if(c1 > 0.0){
		discard;
	}
	else{
		COLOR = vec4(vec3(c1),1.0f);
	}
	

}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
