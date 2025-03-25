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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        base._Ready();

        globalScript = Globals.Instance;

        tutorialButton = GetNode<TutorialButton>("UICanvas/TutorialButton");
        tutorialButton.startID = constellationTutorialStartID;

        // please rename this at some point
        camera = GetNode<CameraMovement>("eeek");
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

        if (globalScript.gameState.stage > GAMESTAGE.CONSTELLATION)
        {
            //Have some indication of winning!
            exitTimer += delta;
            if (exitTimer > 2.5) Close();
        }
        if (dialogueBox.Visible) {
            camera.canMoveCam = false;
        }
    }

    void RegainCameraControl()
    {
        camera.canMoveCam = true;
    }
}
