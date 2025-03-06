using Godot;
using System;
using System.Runtime.InteropServices;

public partial class WipeTransition : CanvasLayer
{
	ColorRect transitionRect;
	AnimationPlayer transitionPlayer;

	ShaderMaterial Smaterial;

	[Export]
	float fadeWith = 300.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		transitionRect = GetChild(0, false) as ColorRect;
		transitionPlayer = GetChild(1,false) as AnimationPlayer; 
		Smaterial = transitionRect.Material as ShaderMaterial;
        Smaterial.SetShaderParameter("fadeWidth", fadeWith);
        //Visible = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Smaterial.SetShaderParameter("currentSize", transitionRect.Size.X);

		//TESTING PURPOSES
		//GD.Print(transitionRect.Size.X);
		//if (Input.IsActionJustPressed("TEST"))
		//{
		//	GD.Print("PLAY ANIM");
		//	transitionPlayer.Play("WipeToRight");
		//}
	}
}
