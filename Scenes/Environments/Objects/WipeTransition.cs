using Godot;
using System;
using System.Runtime.InteropServices;

public partial class WipeTransition : Control
{
	ColorRect transitionRect;
	AnimationPlayer transitionPlayer;

	ShaderMaterial Smaterial;
	[Export]

	float fadeWith = 300.0f;

	float initalTime = 0f;
	static TRANSITION nextTransition = TRANSITION.NONE;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		transitionRect = GetChild(0, false) as ColorRect;
		transitionPlayer = GetChild(1,false) as AnimationPlayer; 
		Smaterial = transitionRect.Material as ShaderMaterial;
        Smaterial.SetShaderParameter("fadeWidth", fadeWith);

        Visible = false;

        if (nextTransition != TRANSITION.NONE) ReverseTransition();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Smaterial.SetShaderParameter("currentSize", transitionRect.Size.X);

		
		
	}

	private void PlayTransition(TRANSITION type)
	{
		GD.Print("WORKED", type.ToString());
        switch (type)
        {
            case TRANSITION.RIGHT:
                Visible = true;
                transitionPlayer.Play("WipeToLeft");
				nextTransition = TRANSITION.LEFT;
                break;
            case TRANSITION.LEFT:
				
				Visible = true;
				transitionPlayer.Play("WipeToRight");
				nextTransition = TRANSITION.RIGHT;
                break;
            case TRANSITION.TOP:

                break;
            case TRANSITION.BOTTOM:
                break;


        }
    }

	private void ReverseTransition()
	{
        switch (nextTransition)
        {
            case TRANSITION.RIGHT:
                Visible = true;
                transitionPlayer.PlayBackwards("WipeToLeft");
                break;
            case TRANSITION.LEFT:

                Visible = true;
                transitionPlayer.PlayBackwards("WipeToRight");
                break;
            case TRANSITION.TOP:

                break;
            case TRANSITION.BOTTOM:
                break;


        }
    }
}
