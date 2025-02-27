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

        // i feel so bad putting this in here (0 cohesion 100 coupling) but im not making a constellationroot script
        // just to put thsi in there. todo: base minigame class? - erf
        if (Input.IsActionJustPressed("close"))
        {
            Close();
        }
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

    void Close()
    {
        Node parent = GetParent();
        Player plr = parent.GetNode<Player>("../Player");
        if (plr != null)
        {
            plr.setMovementState(MovementStates.FREE_MOVE);
            parent.QueueFree();
        }
    }
}
