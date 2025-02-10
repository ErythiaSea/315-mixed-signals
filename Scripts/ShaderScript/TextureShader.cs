using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class TextureShader : ColorRect
{
	ShaderMaterial Smaterial;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CollisionShape2D shape = GetNode<CollisionShape2D>("../Gesture/DrawingCanvas");

        CircleShape2D circleShape = shape.Shape as CircleShape2D;

        if (circleShape != null && shape != null)
		{
            Smaterial = this.Material as ShaderMaterial;

			
            Smaterial.SetShaderParameter("circle_r", circleShape.Radius);
            Smaterial.SetShaderParameter("circle_c", shape.Position);
        }
		else
		{
			GD.Print("Shader shape is null");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
