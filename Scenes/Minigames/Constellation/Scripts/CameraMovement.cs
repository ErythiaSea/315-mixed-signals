using Godot;
using System;

public partial class CameraMovement : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        CameraInputEvents(delta);
	}

	public void CameraInputEvents(double delta)
	{
        if (Input.IsActionPressed("down"))
        {
			Position = new Vector2(Position.X, Position.Y + 0.3f);
        }
		if (Input.IsActionPressed("up"))
		{
            Position = new Vector2(Position.X, Position.Y - 0.3f);
        }
        if (Input.IsActionPressed("left"))
        {
            Position = new Vector2(Position.X - 0.3f, Position.Y);
        }
        if (Input.IsActionPressed("right"))
        {
            Position = new Vector2(Position.X + 0.3f, Position.Y);
        }
    }
}
