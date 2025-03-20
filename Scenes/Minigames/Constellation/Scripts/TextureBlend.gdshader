shader_type canvas_item;


uniform sampler2D texture_1;
uniform bool isFound;
void vertex() {
	
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.

	if(isFound){
		vec4 basePix = texture(TEXTURE,UV);
		vec4 newPix = texture(texture_1,UV);

		COLOR.rgb = mix(basePix.rgb,newPix.rgb,newPix.a);
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
