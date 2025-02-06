using Godot;
using System;

public partial class RestrictionBox : Area2D
{
	[Export]
	bool RestrictHorizontal = false;
	[Export]
	bool RestrictVertical = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (Node2D body in GetOverlappingBodies()) {
			Player plr = body as Player;
			if (plr != null)
			{
				plr.restrictHorizontal = RestrictHorizontal;
				plr.restrictVertical = RestrictVertical;
			}
		}
	}
}
