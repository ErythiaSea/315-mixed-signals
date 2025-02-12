using Godot;
using System;

public partial class ConstellationShader : Sprite2D
{
	ShaderMaterial Smaterial;
	Texture RenderTexture;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Smaterial = this.Material as ShaderMaterial;
		RenderTexture = ImageTexture.CreateFromImage(GetViewport().GetTexture().GetImage());

        Smaterial.SetShaderParameter("viewportRT", RenderTexture);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
