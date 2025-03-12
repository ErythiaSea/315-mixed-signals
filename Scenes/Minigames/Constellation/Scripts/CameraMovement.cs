using Godot;
using System;

public partial class CameraMovement : Camera2D
{
    [Export]
    public float cameraSpeed = 0.6f;
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
			Position = new Vector2(Position.X, Position.Y + cameraSpeed);
        }
		if (Input.IsActionPressed("up"))
		{
            Position = new Vector2(Position.X, Position.Y - cameraSpeed);
        }
        if (Input.IsActionPressed("left"))
        {
            Position = new Vector2(Position.X - cameraSpeed, Position.Y);
        }
        if (Input.IsActionPressed("right"))
        {
            Position = new Vector2(Position.X + cameraSpeed, Position.Y);
        }
    }
}
