using Godot;
using System;

public partial class Reset : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("CameraDown"))
		{
			PackedScene debugroom = ResourceLoader.Load<PackedScene>("res://Scenes/debugroom.tscn");
            GetTree().ChangeSceneToPacked(debugroom);
        }
	}
}
