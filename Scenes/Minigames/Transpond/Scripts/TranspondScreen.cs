using Godot;
using System;

public partial class TranspondScreen : BaseMinigame  
{
    Radiotower radiotower;
    WaveformGame waveform;
    Sprite2D leftBox, rightBox;
    Label radioLabel, waveLabel;

    bool radiotowerComplete = false; bool waveformComplete = false;
    double exitTimer = 0;
    bool fade = false; float fadeTime = 0.0f;

    Globals globalScript;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        globalScript = GetTree().Root.GetChild(1) as Globals;

        radiotower = GetNode<Radiotower>("radiotowerRoot");
        waveform = GetNode<WaveformGame>("waveformRoot");
        leftBox = GetNode<Sprite2D>("LeftBox");
        rightBox = GetNode<Sprite2D>("RightBox");
        radioLabel = GetNode<Label>("ControlsRadiotower");
        waveLabel = GetNode<Label>("ControlsWaveform");

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

            GD.Print("winner!");
            //Have some indication of winning!
        }
    }  

    private void CheckStage()
    {
        switch (globalScript.gameState.stage)
        {
            case GAMESTAGE.TRANSPONDING:
                GD.Print("trans");
                break;
            case GAMESTAGE.WAVEFORM:
                GD.Print("wave");
                radiotower.CompletedPivots();
                waveLabel.Visible = true; radioLabel.Visible = false;
                fade = true;
                break;
            default:
                GD.Print("Default");
                radiotower.CompletedPivots();
                break;
        }
    }
}
