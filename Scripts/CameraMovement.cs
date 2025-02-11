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
        if (Input.IsActionPressed("CameraDown"))
        {
			Position = new Vector2(Position.X, Position.Y + 10);
        }
		if (Input.IsActionPressed("CameraUp"))
		{
            Position = new Vector2(Position.X, Position.Y - 10);
        }
        if (Input.IsActionPressed("CameraLeft"))
        {
            Position = new Vector2(Position.X - 10, Position.Y);
        }
        if (Input.IsActionPressed("CameraRight"))
        {
            Position = new Vector2(Position.X + 10, Position.Y);
        }
    }
}
