shader_type canvas_item;
uniform float fadeWidth : hint_range(0.0,300.0);
uniform float currentSize : hint_range (0.0,2300.0);
void vertex() {
	// Called for every vertex the material is visible on.
	
}

void fragment() {
	// Called for every pixel the material is visible on.
	float rectUV = UV.x * 2300.0;

   	float fadeStart = currentSize - fadeWidth;
    float fade = smoothstep(currentSize, fadeStart, rectUV);
	
	
	
	if(rectUV > fadeStart){
		COLOR = vec4(0.0, 0.0, 0.0, fade);
	}
	
    // Black color with fading alpha
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
