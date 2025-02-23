shader_type canvas_item;
uniform bool isLeft;
uniform float progress : hint_range(0.0, 1.0);
uniform vec4 glow_color = vec4(1.0,1.0,0.5,1.0);
uniform float glow_intensity : hint_range(0.0, 10.0) = 3.0;

vec4 ApplyColor(vec2 uv, sampler2D tex){
		vec4 color = texture(tex,uv);
        vec4 glow = glow_color * glow_intensity;
        color.rgb += glow.rgb; // Add glow effect
        return color;
}

void vertex() {
	// Called for every vertex the material is visible on.
}


void fragment() {
	// Called for every pixel the material is visible on.
	vec2 uv = UV;
    
    // If the pixel is within the progress range, apply glow
	if(isLeft){
		if (uv.x <= progress) 
		{
        	COLOR = ApplyColor(uv,TEXTURE);
    	} else {
        	discard; // Hide unused parts
    	}
	}
	else{
		if(uv.x >= progress){
			COLOR = ApplyColor(uv,TEXTURE);
		}
		else{
			discard;
		}
	}
}
//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
