using Godot;
using System;

public partial class ConstellationMinigame : BaseMinigame
{
	// The start ID used for the tutorial dialogue
	[Export] String constellationTutorialStartID = "3";
	[Export] String constellationEndStartID = "4";

	double exitTimer = 0;

	//references too objects in constellation scene
	private Panel dialogueBox;
	private TutorialButton tutorialButton;
	private CameraMovement camera;
	private StarsParent constellationInstance;
	private Godot.Collections.Array<Node2D> constellations;

	// mmmmmm hardcode path :vmunch:
	private const string outdoorCabinPath = "res://Scenes/Environments/CabinOutdoor/cabinoutdoor.tscn";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Globals.PushGamestate(GAMESTATE.CONSTELLATION);

		tutorialButton = GetNode<TutorialButton>("UICanvas/TutorialButton");
		tutorialButton.startID = constellationTutorialStartID;

		camera = GetNode<CameraMovement>("Camera");


		constellations = GetConstellations();
		SetupConstellation();

		//setting the constellation based on the day the game is on i.e day 0: Pyxis, day 1: Corvus
		constellationInstance = constellations[Globals.Day] as StarsParent;
		constellationInstance.ConstellationCompletion += camera.DisplayConstellation;

		//Shows necessary tutorial boxes
		dialogueBox = GetNode<Panel>("UICanvas/DialogueBox");
		if (Globals.TutorialProgress <= GAMESTAGE.CONSTELLATION)
		{
			dialogueBox.Call("start", constellationTutorialStartID);
			Globals.TutorialProgress = GAMESTAGE.TRANSLATION;
		}

		dialogueBox.Connect("dialogue_ended", Callable.From(RegainCameraControl));
		AudioServer.SetBusMute(AudioServer.GetBusIndex("Environmental"), true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		//if any dialogue is shown then the camera cant be moved until dialogue completed
		if (dialogueBox.Visible)
		{
			camera.canMoveCam = false;
		}
	}

	//Needs commented but i will leave this for now if its to be axed - Kyle
	// todo: remove circular relationship between cam and parent and we can maybe axe this - eryth
	public void ShowFinalBox()
	{
		Globals.ProgressionStage = GAMESTAGE.TRANSLATION;

		ResourceLoader.LoadThreadedRequest(outdoorCabinPath);
		Globals.CurrentSpawnID = 1;
		GD.Print("loading cabin outdoor...");

		// change dialogue called based on day
		dialogueBox.Call("start", (constellationEndStartID + Globals.Day.ToString()));
		dialogueBox.Connect("dialogue_ended", Callable.From(Close));
	}

	//Allows camera control
	void RegainCameraControl()
	{
		camera.canMoveCam = true;
	}

	//Uncommented as it needs to be updated to find the constellations in a better way, this is gross dude - me - kyle {
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

	//Sets up the current days constellation to be played
	private void SetupConstellation()
	{
		constellations[Globals.Day].Visible = true;
		StarsParent prt = constellations[Globals.Day] as StarsParent;
		prt.GenerateNumbers();
	}

	//Uncommented 
	protected override void OnTransitionFinish()
	{
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Environmental"), false);
        Globals.PopGamestate(GAMESTATE.CONSTELLATION);
		// only load a new scene if constellation was actually completed
		if (Globals.ProgressionStage < GAMESTAGE.TRANSLATION)
		{ 
			return; 
		}

		PackedScene cabin = (PackedScene)ResourceLoader.LoadThreadedGet(outdoorCabinPath);
		if (cabin == null) return;
	
		// i should be shot
		// this is a hack fix for the transition animation not playing when the next scene loads, because the
		// scene loads after the transition finish signal is emitted, but also after the next transition starts
		// playing. this sets the right transition state (bottom to top reverse) for the newly loaded scene
		// we should have just made this a singleton - eryth
		player.EmitSignal(Player.SignalName.Transition, (int)TRANSITION.TOPtoBOTTOM, 1.0f);
		GetTree().ChangeSceneToPacked(cabin);
	}
}
