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
        }

		if (Input.IsActionJustPressed("TEST"))
		{
			Globals.ProgressionStage++;
			GD.Print("stage increased to ", Globals.ProgressionStage);

			if (Globals.ProgressionStage == GAMESTAGE.TRANSLATION)
			{
				TranslationCanvasUI.CipherKey = 6;
			}
		}

		// close when holding esc (todo maybe get rid of?)
		if (Input.IsActionPressed("close"))
		{
            killtimer += delta;
			if (killtimer > 0.5)
			{
				GD.Print("kill");
				GetTree().Quit();
			}
		}
		else killtimer = 0;
	}
}
