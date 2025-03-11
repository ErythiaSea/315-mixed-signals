shader_type canvas_item;
uniform float fadeWidth;
uniform float currentSize : hint_range (0.0,2300.0);
uniform bool isHorizontal;
void vertex() {
	// Called for every vertex the material is visible on.
	
}

void fragment() {
	// Called for every pixel the material is visible on.
	
	if(isHorizontal){
		float rectUV = UV.x * 2300.0;
   		float fadeStart = currentSize - fadeWidth;
    	float fade = smoothstep(currentSize, fadeStart, rectUV);
	
		if(rectUV > fadeStart){
			COLOR = vec4(0.0, 0.0, 0.0, fade);
		}
	}
	else{
		float rectUV = UV.y * 1403.0;
   		float fadeStart = currentSize - fadeWidth;
    	float fade = smoothstep(currentSize, fadeStart, rectUV);
	
		if(rectUV > fadeStart){
			COLOR = vec4(0.0, 0.0, 0.0, fade);
		}
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
