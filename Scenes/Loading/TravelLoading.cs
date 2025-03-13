using Godot;
using System;

public partial class TravelLoading : Node2D
{
	[Export]
	float loadLength = 3.0f;
    private float loadingTimer = 0.0f;
	
	// todo: retool baseminigame so i dont have to keep doing this - erf
	[Export]
	float transitionLength = 1.0f;
	private float transitionTimer = 0.0f;
	private bool inTransition = false;

	private WipeTransition transition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		transition = GetNode<WipeTransition>("WipeTransition");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (inTransition)
		{
			transitionTimer += (float)delta;
			if (transitionTimer > transitionLength)
			{
                GetTree().ChangeSceneToPacked(Globals.Instance.nextMap);
            }
			return;
		}
		loadingTimer += (float)delta;
		if (loadingTimer > loadLength)
		{
			transition.PlayTransition(TRANSITION.LEFTtoRIGHT, transitionLength);
			inTransition = true;
		}
	}
}
