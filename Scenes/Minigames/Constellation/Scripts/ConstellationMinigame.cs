using Godot;
using System;

public partial class ConstellationMinigame : BaseMinigame
{
    // The start ID used for the tutorial dialogue
    [Export] String constellationTutorialStartID = "3";
    [Export] String constellationEndStartID = "4";

    double exitTimer = 0;
    Globals globalScript;

    Panel dialogueBox;
    TutorialButton tutorialButton;
    CameraMovement camera;
    StarsParent starsParent;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        base._Ready();

        globalScript = Globals.Instance;

        tutorialButton = GetNode<TutorialButton>("UICanvas/TutorialButton");
        tutorialButton.startID = constellationTutorialStartID;

        // please rename this at some point
        camera = GetNode<CameraMovement>("eeek");

        starsParent = GetNode<StarsParent>("Constellation");
        starsParent.ConstellationCompletion += camera.DisplayConstellation;

        dialogueBox = GetNode<Panel>("UICanvas/DialogueBox");
        if (Globals.Instance.tutorialProgress <= GAMESTAGE.CONSTELLATION)
        {
            dialogueBox.Call("start", constellationTutorialStartID);
            Globals.Instance.tutorialProgress = GAMESTAGE.TRANSLATION;
        }

        dialogueBox.Connect("dialogue_ended", Callable.From(RegainCameraControl));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        base._Process(delta);

        if (dialogueBox.Visible) {
            camera.canMoveCam = false;
        }
    }

    // remove circular relationship between cam and parent and we can maybe axe this - eryth
    public void ShowFinalBox()
    {
        globalScript.gameState.stage = GAMESTAGE.TRANSLATION;
        dialogueBox.Call("start", constellationEndStartID);
        dialogueBox.Connect("dialogue_ended", Callable.From(Close));
    }

    void RegainCameraControl()
    {
        camera.canMoveCam = true;
    }
}
