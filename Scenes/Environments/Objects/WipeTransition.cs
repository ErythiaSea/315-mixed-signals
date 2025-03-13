using Godot;
using System;
using System.Runtime.InteropServices;

public partial class WipeTransition : CanvasLayer
{
	ColorRect transitionRect;
	AnimationPlayer transitionPlayer;

	ShaderMaterial Smaterial;
    [Export]

    bool isHorizontal;
	float fadeWidth = 300.0f;

	float initalTime = 0f;
	static TRANSITION nextTransition = TRANSITION.NONE;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		transitionRect = GetChild(0, false) as ColorRect;
		transitionPlayer = GetChild(1,false) as AnimationPlayer; 
		Smaterial = transitionRect.Material as ShaderMaterial;
        Smaterial.SetShaderParameter("fadeWidth", fadeWidth);

        if (nextTransition != TRANSITION.NONE) ReverseTransition();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Smaterial.SetShaderParameter("isHorizontal", isHorizontal);
        if (isHorizontal)
        {
            Smaterial.SetShaderParameter("currentSize", transitionRect.Size.X);
        }
        else 
        {
            Smaterial.SetShaderParameter("currentSize", transitionRect.Size.Y);
        }
	}

	private void PlayTransition(TRANSITION type, float transitionLength)
	{
        transitionPlayer.SpeedScale = (1.0f / transitionLength);
		Visible = true;

		switch (type)
        {
        case TRANSITION.RIGHTtoLEFT:
            isHorizontal = true;
            transitionPlayer.Play("WipeToLeft");
			nextTransition = TRANSITION.LEFTtoRIGHT;
            break;
        case TRANSITION.LEFTtoRIGHT:
            isHorizontal = true;
			transitionPlayer.Play("WipeToRight");
			nextTransition = TRANSITION.RIGHTtoLEFT;
            break;
        case TRANSITION.TOPtoBOTTOM:
            isHorizontal = false;
            transitionPlayer.Play("WipeToBottom");
            nextTransition = TRANSITION.BOTTOMtoTOP;
            break;
        case TRANSITION.BOTTOMtoTOP:
            isHorizontal = false;
            transitionPlayer.Play("WipeToTop");
            nextTransition = TRANSITION.TOPtoBOTTOM;
            break;
        }
	}

	private void ReverseTransition()
	{
		Visible = true;

		switch (nextTransition)
        {
        case TRANSITION.RIGHTtoLEFT:
            isHorizontal = true;
            transitionPlayer.PlayBackwards("WipeToLeft");
            break;
        case TRANSITION.LEFTtoRIGHT:
            isHorizontal = true;
            transitionPlayer.PlayBackwards("WipeToRight");
            break;
        case TRANSITION.TOPtoBOTTOM:
            isHorizontal = false;
            transitionPlayer.PlayBackwards("WipeToBottom");
            break;
        case TRANSITION.BOTTOMtoTOP:
            isHorizontal = false;
            transitionPlayer.PlayBackwards("WipeToTop");
            break;
        }
        nextTransition = TRANSITION.NONE;
	}

    private void OnAnimationEnd(StringName animName)
    {
        if (nextTransition == TRANSITION.NONE) return;
        ReverseTransition();
        //nextTransition = TRANSITION.NONE;
    }
}
