using Godot;
using System;

// todo: the fact that any of this exists makes me profusely sad
// it shouldnt but it does. it might be too late to get rid of anyway. the damage is done - eryth
public partial class CabinLevel : Level
{
	// the dialogue box where post-translation text shows up
	[Export]
	Control dialogueBox;
	// the start id for post-translation dialogue
	[Export]
	String translationEndStartID = "translationend";

	// the interact box that will summon the game end screen on day 3
	[Export]
	InteractBox exitInteractBox;
	// the end game screen
	private string endScreenPath = "res://Scenes/Menu/GameEnd/game_end_temp.tscn";
	// elevator button so it can be disabled on day 3
	[Export]
	InteractBox elevatorButtonBox;

	EndDay endDay;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();
		endDay = GetNode<EndDay>("EndDay");
        Globals.Instance.DayChanged += OnNewDay;
	}

    private void OnNewDay()
    {
        if (Globals.Day == 2)
		{
			GD.Print("Final day, so swapping the level change scene and disabling elevator...");
			exitInteractBox.ChangeLoadedScene(endScreenPath);
			elevatorButtonBox.QueueFree(); // nuclear approach is, sometimes, the best
			elevatorButtonBox = null;
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public void TranslationComplete()
	{
		GD.Print("translation complete");
		if (Globals.ProgressionStage == GAMESTAGE.END)
		{
			dialogueBox.Call("start", translationEndStartID);
		}
	}
}
