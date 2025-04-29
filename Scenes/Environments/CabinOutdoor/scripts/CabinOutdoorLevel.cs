using Godot;
using System;

public partial class CabinOutdoorLevel : Level
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		if (Globals.Day == 2 || Globals.ProgressionStage > GAMESTAGE.CONSTELLATION)
		{
			GetNode<CollisionShape2D>("LevelCollision/carWall").Disabled = false;
		}
		else
		{
            GetNode<CollisionShape2D>("LevelCollision/carWall").Disabled = true;
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
