using Godot;
using System;

public partial class GodsDisplay : Node2D
{
	InteractBox box;
	CanvasLayer godsCanvas;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		box = GetNode("GodBox") as InteractBox;
		godsCanvas = GetNode<CanvasLayer>("GodsCanvas");
		if (Globals.Day == 2)
		{
			Visible = true;
			box.active = true;
            godsCanvas.Visible = true;
        }
		else
		{
			Visible = false;
			box.active = false;
			godsCanvas.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
