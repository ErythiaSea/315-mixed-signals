using Godot;
using System;

public partial class TranspondScreen : BaseMinigame  
{
    Radiotower radiotower;
    WaveformGame waveform;
    Sprite2D leftBox, rightBox;
    Label radioLabel, waveLabel;
    Panel dialogueBox;

    bool radiotowerComplete = false; bool waveformComplete = false;
    double exitTimer = 0;
    bool fade = false; float fadeTime = 0.0f;
    bool dialogueCalled = false;

    Globals globalScript;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
        globalScript = GetTree().Root.GetChild(1) as Globals;

        radiotower = GetNode<Radiotower>("radiotowerRoot");
        waveform = GetNode<WaveformGame>("waveformRoot");
        leftBox = GetNode<Sprite2D>("LeftBox");
        rightBox = GetNode<Sprite2D>("RightBox");
        radioLabel = GetNode<Label>("ControlsRadiotower");
        waveLabel = GetNode<Label>("ControlsWaveform");

        // temp hopefully
        dialogueBox = GetNode<Panel>("DialogueBox");

        CheckStage();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (globalScript.gameState.stage == GAMESTAGE.WAVEFORM)
        {
            waveLabel.Visible = true; radioLabel.Visible = false;
            fade = true;
            if (!dialogueCalled)
            {
                dialogueBox.Call("start", "1");
                dialogueCalled = true;
            }
        }

		if (fade)
		{
			fadeTime += (float)delta;
			if (fadeTime >= 0.85f) { fade = false; fadeTime = 0.85f; }
			leftBox.Modulate = new Color(Colors.Black, fadeTime);
			rightBox.Modulate = new Color(Colors.Black, 0.85f - fadeTime);
		}

        if (globalScript.gameState.stage > GAMESTAGE.WAVEFORM)
        {
            //Have some indication of winning!
            exitTimer += delta;
            if (exitTimer > 2.5) Close();
        }
    }  

    private void CheckStage()
    {
        switch (globalScript.gameState.stage)
        {
            case GAMESTAGE.TRANSPONDING:
                GD.Print("trans");
                dialogueBox.Call("start", "0");
                break;
            case GAMESTAGE.WAVEFORM:
                GD.Print("wave");
                radiotower.CompletedPivots();
                waveLabel.Visible = true; radioLabel.Visible = false;
                fade = true;
                dialogueBox.Call("start", "1");
                break;
            default:
                GD.Print("Default");
                radiotower.CompletedPivots();
                break;
        }
    }
}
