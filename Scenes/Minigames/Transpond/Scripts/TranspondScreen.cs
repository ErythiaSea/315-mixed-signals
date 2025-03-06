using Godot;
using System;

public partial class TranspondScreen : Node2D
{
    Radiotower radiotower;
    WaveformGame waveform;
    Sprite2D leftBox, rightBox;
    Label radioLabel, waveLabel;

    bool radiotowerComplete = false; bool waveformComplete = false;
    double exitTimer = 0;
    bool fade = false; float fadeTime = 0.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        radiotower = GetNode<Radiotower>("radiotowerRoot");
        waveform = GetNode<WaveformGame>("waveformRoot");
        leftBox = GetNode<Sprite2D>("LeftBox");
        rightBox = GetNode<Sprite2D>("RightBox");
        radioLabel = GetNode<Label>("ControlsRadiotower");
        waveLabel = GetNode<Label>("ControlsWaveform");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (!radiotower.gameActive && !radiotowerComplete)
        {
            waveform.gameActive = true;
            radiotowerComplete = true;
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

        if (!waveform.gameActive && radiotowerComplete)
        {
            waveformComplete = true;
            waveform.gameActive = false;
            GD.Print("winner!");
        }
        if (radiotowerComplete && waveformComplete)
        {
            exitTimer += delta;
            if (exitTimer > 1)
            {
                Close();
            }
        }

        if (Input.IsActionJustPressed("close"))
        {
            Close();
        }
        if (Input.IsActionJustPressed("arrow_up"))
        {
            waveform.gameActive = true;
            radiotowerComplete = true;
            radiotower.gameActive = false;
        }
    }

    public void Close()
    {
		Player plr = GetNode<Player>("../Player");
        plr.SetMovementLock(false);
        QueueFree();
    }
}
