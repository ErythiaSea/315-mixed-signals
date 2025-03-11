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
        switch (type)
        {
            case TRANSITION.RIGHTtoLEFT:
                Visible = true;
                transitionPlayer.Play("WipeToLeft");
				nextTransition = TRANSITION.LEFTtoRIGHT;
                break;
            case TRANSITION.LEFTtoRIGHT:
				
				Visible = true;
				transitionPlayer.Play("WipeToRight");
				nextTransition = TRANSITION.RIGHTtoLEFT;
                break;
            case TRANSITION.TOPtoBOTTOM:

                break;
            case TRANSITION.BOTTOMtoTOP:
                break;


        }
    }

	private void ReverseTransition()
	{
        switch (nextTransition)
        {
            case TRANSITION.RIGHTtoLEFT:
                Visible = true;
                transitionPlayer.PlayBackwards("WipeToLeft");
                break;
            case TRANSITION.LEFTtoRIGHT:

                Visible = true;
                transitionPlayer.PlayBackwards("WipeToRight");
                break;
            case TRANSITION.TOPtoBOTTOM:

                break;
            case TRANSITION.BOTTOMtoTOP:
                break;


        }
    }
}
