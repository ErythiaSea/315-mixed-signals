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
			GD.Print("day: ", Globals.Day);
			GD.Print("state: ", Globals.Gamestate);
			GD.Print("stage: ", Globals.ProgressionStage);
		}

		if (Input.IsActionJustPressed("TEST") && Input.IsActionPressed("arrow_down") && Input.IsActionPressed("arrow_up"))
		{
			Globals.ProgressionStage++;
			GD.Print("stage increased to ", Globals.ProgressionStage);

			if (Globals.ProgressionStage == GAMESTAGE.TRANSLATION)
			{
				TranslationCanvasUI.CipherKey = 6;
			}
		}
	}
}
