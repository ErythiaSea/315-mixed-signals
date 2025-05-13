using Godot;
using System;

// todo: the fact that any of this exists makes me profusely sad
// it shouldnt but it does. it might be too late to get rid of anyway. the damage is done - eryth
public partial class CabinLevel : Level
{
	// the dialogue box where post-translation text shows up
	[Export]
	Control dialogueBox;

	[Export]
	Control dialogueBubble;

	// the start id for post-translation dialogue
	[Export]
	String translationEndStartID = "translationend";
	// the start id for post-photoboard dialogue on day 1
	[Export]
	String photoboardCloseStartID = "photoboardclose";

	// the interact box that will summon the game end screen on day 3
	[Export]
	InteractBox exitInteractBox;
	// the end game screen
	private string endScreenPath = "res://Scenes/Menu/GameEnd/game_end_temp.tscn";
	// elevator button so it can be disabled on day 3
	[Export]
	InteractBox elevatorButtonBox;

	EndDay endDay;

	AudioStreamPlayer sceneMusic;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		sceneMusic = GetNode("SceneMusic") as AudioStreamPlayer;
		endDay = GetNode<EndDay>("EndDay");

		Globals.Instance.DayChanged += OnNewDay;
		Globals.Instance.ProgressionChange += OnGamestageIncrease;

		GetNode<InteractBox>("BedBox").Interacted += endDay.EndTheDay;
	}

	private void OnNewDay()
	{
		GetNode<InteractBox>("TranspondBox").active = true;
		if (Globals.Day == 2)
		{
			GD.Print("Final day, so swapping the level change scene and disabling elevator AND translation...");
			//exitInteractBox.ChangeLoadedScene(endScreenPath);
			elevatorButtonBox.QueueFree(); // nuclear approach is, sometimes, the best
			elevatorButtonBox = null;
			GetNode<InteractBox>("TranslationBox").active = false;
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
			dialogueBubble.Call("start", translationEndStartID);
		}
	}

	public void PhotoboardClosed()
	{
		GD.Print("photoboard closed");
		if (Globals.ProgressionStage == GAMESTAGE.BEGIN)
		{
			Globals.ProgressionStage = GAMESTAGE.TRANSPONDING;
			dialogueBubble.Call("start", photoboardCloseStartID);
		}
	}

	private void OnGamestageIncrease()
	{
		if (Globals.ProgressionStage > GAMESTAGE.WAVEFORM)
		{
			GetNode<InteractBox>("TranspondBox").active = false;
		}
	}

	// Disconnect custom signals as they won't be automatically disconnected
	public override void _ExitTree()
	{
		base._ExitTree();
		Globals.Instance.ProgressionChange -= OnGamestageIncrease;
		Globals.Instance.DayChanged -= OnNewDay;
	}
}
