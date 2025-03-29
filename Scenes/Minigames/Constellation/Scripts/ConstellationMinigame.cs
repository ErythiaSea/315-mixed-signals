using Godot;
using System;

public partial class ConstellationMinigame : BaseMinigame
{
	// The start ID used for the tutorial dialogue
	[Export] String constellationTutorialStartID = "3";
	[Export] String constellationEndStartID = "4";

	double exitTimer = 0;

	Panel dialogueBox;
	TutorialButton tutorialButton;
	CameraMovement camera;
	StarsParent starsParent;
	Godot.Collections.Array<Node2D> constellations;

	// mmmmmm hardcode path :vmunch:
	private const string outdoorCabinPath = "res://Scenes/Environments/CabinOutdoor/cabinoutdoor.tscn";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		constellations = GetConstellations();

		SetupConstellation();

		tutorialButton = GetNode<TutorialButton>("UICanvas/TutorialButton");
		tutorialButton.startID = constellationTutorialStartID;

		// please rename this at some point
		camera = GetNode<CameraMovement>("eeek");

		starsParent = constellations[Globals.Day] as StarsParent;
        starsParent.ConstellationCompletion += camera.DisplayConstellation;

		dialogueBox = GetNode<Panel>("UICanvas/DialogueBox");
		if (Globals.TutorialProgress <= GAMESTAGE.CONSTELLATION)
		{
			dialogueBox.Call("start", constellationTutorialStartID);
			Globals.TutorialProgress = GAMESTAGE.TRANSLATION;
		}

		dialogueBox.Connect("dialogue_ended", Callable.From(RegainCameraControl));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (dialogueBox.Visible)
		{
			camera.canMoveCam = false;
		}
	}

	// todo: remove circular relationship between cam and parent and we can maybe axe this - eryth
	public void ShowFinalBox()
	{
		Globals.ProgressionStage = GAMESTAGE.TRANSLATION;

		ResourceLoader.LoadThreadedRequest(outdoorCabinPath);
		Globals.CurrentSpawnID = 1;
		GD.Print("loading cabin outdoor...");

		dialogueBox.Call("start", constellationEndStartID);
		dialogueBox.Connect("dialogue_ended", Callable.From(Close));
	}

	void RegainCameraControl()
	{
		camera.canMoveCam = true;
	}

	private Godot.Collections.Array<Node2D> GetConstellations()
	{
		Godot.Collections.Array<Node2D> consts = new Godot.Collections.Array<Node2D>();
		foreach (Node child in GetChildren())
		{
			if (child.Name == "Corvus" || child.Name == "Pyxis")
			{
				consts.Add(child as Node2D);
			}
		}

		return consts;
	}

	private void SetupConstellation()
	{
		constellations[Globals.Day].Visible = true;
		StarsParent prt = constellations[Globals.Day] as StarsParent;
		prt.GenerateNumbers();
	}

	protected override void OnTransitionFinish()
	{
		PackedScene cabin = (PackedScene)ResourceLoader.LoadThreadedGet(outdoorCabinPath);
		if (cabin != null)
		{
			// i should be shot
			player.EmitSignal(Player.SignalName.Transition, (int)TRANSITION.TOPtoBOTTOM, 1.0f);
			GetTree().ChangeSceneToPacked(cabin);
		}
	}
}
