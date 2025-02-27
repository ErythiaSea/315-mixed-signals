using Godot;
using System;
using static Godot.HttpClient;
using System.Reflection;

public partial class Reset : Node
{
	double killtimer = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// load debug on down+up arrow
		if (Input.IsActionJustPressed("arrow_down") && Input.IsActionPressed("arrow_up"))
		{
            PackedScene debugroom = ResourceLoader.Load<PackedScene>("res://Scenes/debugroom.tscn");
            GetTree().ChangeSceneToPacked(debugroom);
        }

		// close when holding esc
		if (Input.IsActionPressed("close"))
		{
			killtimer += delta;
			if (killtimer > 0.5)
			{
				GetTree().Quit();
			}
		}
		else killtimer = 0;
	}
}
