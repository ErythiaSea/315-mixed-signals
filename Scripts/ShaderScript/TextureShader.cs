using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

public partial class TextureShader : ColorRect
{
	ShaderMaterial Smaterial;

	//Radius of telescope lens
	[Export]
	public float radius;

	//interpolation value for smoothness between lens and lens edge
	[Export]
	public float blur;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//sets the shader variables for telescope lens creation
		Smaterial = this.Material as ShaderMaterial;
        Smaterial.SetShaderParameter("circle_r", radius);
        Smaterial.SetShaderParameter("circle_b", blur);
			
	}
}
