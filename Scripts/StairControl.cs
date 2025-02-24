using Godot;
using System;

public partial class StairControl : Area2D
{
	// The attached stair is the one that goes up from this area!!
	[Export]
	CollisionShape2D stairToToggle;
	[Export]
	bool downByDefault = false;

	GravPlayer player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<GravPlayer>("../GravPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		stairToToggle.Disabled = false;
		if (OverlapsBody(player))
		{
			if (Input.IsActionPressed("down") || downByDefault)
			{
				stairToToggle.Disabled = true;
			}
            if (Input.IsActionPressed("up"))
            {
                stairToToggle.Disabled = false;
            }
        }
	}
}
