using Godot;
using System;

public partial class Cabin : Node2D
{
	Globals globalScript;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globalScript = GetTree().Root.GetChild(1) as Globals;

		if (!globalScript.hasGameStarted)
		{
			globalScript.hasGameStarted = true;
			globalScript.gameState.stage = GAMESTAGE.INITAL;
			globalScript.gameState.day = 0;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
