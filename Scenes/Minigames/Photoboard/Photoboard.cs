using Godot;
using System;

public partial class Photoboard : BaseMinigame
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		// set stage to transponding from begin
		if (Globals.Instance.gameState.stage == GAMESTAGE.BEGIN)
		{
			Globals.Instance.gameState.stage = GAMESTAGE.TRANSPONDING;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
