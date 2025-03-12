using Godot;
using System;

public partial class BaseMinigame : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("close")) { Close(); }
	}

	// still doing this a crappy way for now but i'd like to do signal bus later
    protected void Close()
    {
        Player plr = GetNode<Player>("../Player");
        plr.SetMovementLock(false);
        QueueFree();
    }
}
