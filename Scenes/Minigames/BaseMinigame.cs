using Godot;
using System;
using static Godot.Tween;

public partial class BaseMinigame : CanvasLayer
{
	// type of transition this minigame uses on exit
	[Export]
	public TRANSITION exitTransition = TRANSITION.NONE;

	// how long the transition takes
	[Export]
	public float transitionLength = 1.0f;

	// whether this minigame is able to be closed
	protected bool canClose = true;
	// whether this minigame is able to be controlled
	protected bool canHandleInputs = true;

	// if the minigame is mid transition, and for how long
	protected bool inTransition = false;
	protected float transitionTimer = 0.0f;

	protected Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// todo: stop doing this - erf
		player = GetNode<Player>("../Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (inTransition)
		{
			transitionTimer += (float)delta;
			if (transitionTimer > transitionLength) {
				OnTransitionFinish();
				QuitMinigame();
			}
			return;
		}

		if (Input.IsActionJustPressed("close")) 
		{
			Close();
		}
	}

	protected void Close()
	{
		if (!canClose)
		{
			GD.Print("tried to close, but failed");
			return;
		}

		if (exitTransition != TRANSITION.NONE)
		{
			inTransition = true;
			player.EmitSignal("Transition", (int)exitTransition, transitionLength);
		}
		else
		{
			OnTransitionFinish();
			QuitMinigame();
		}
	}

	// still doing this a crappy way for now but i'd like to do signal bus later
	private void QuitMinigame()
	{
		player.SetMovementLock(false);
		player.SetCameraEnabled(true);
		QueueFree();
	}

	protected virtual void OnTransitionFinish()
	{
	}
}
