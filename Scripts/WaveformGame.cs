using Godot;
using System;

public partial class WaveformGame : Node2D
{
	WaveRender playerWave, realWave;
    float targetWavelength, targetAmplitude;

    [Export]
    float ampTolerance = 2.0f;
    [Export]
    float wlTolerance = 2.0f;
    float alignedTimer = 0.0f;
    bool tunedSignal = false;

    public bool gameActive = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerWave = GetNode<WaveRender>("playerwave");
        realWave = GetNode<WaveRender>("realwave");
        newWavelength();
    }

    void newWavelength()
    {
        targetWavelength = (GD.Randf() * 42.5f) + 7.5f;
        realWave.wavelength = targetWavelength;

        targetAmplitude = (GD.Randf() * 115.0f) + 10.0f;
        realWave.amplitude = targetAmplitude;
        tunedSignal = false; alignedTimer = 0.0f;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (!gameActive) return;

        float wlChange = 0.5f;
        float wlMult = (playerWave.wavelength / 100.0f);
        if (Input.IsActionPressed("left_pivot_ccw"))
        {
            playerWave.amplitude += 0.5f;
        }
        if (Input.IsActionPressed("left_pivot_cw"))
        {
            playerWave.amplitude -= 0.5f;
        }
        if (Input.IsActionPressed("right_pivot_ccw"))
        {
            playerWave.wavelength += wlChange * wlMult;
            //playerWave.frequency += wlChange;
        }
        if (Input.IsActionPressed("right_pivot_cw"))
        {
            playerWave.wavelength -= wlChange * wlMult;
            //playerWave.frequency -= wlChange;
        }

        if (playerWave.amplitude < 0.0f) playerWave.amplitude = 0.0f;
        if (Input.IsActionJustPressed("print_intersect"))
        {
            GD.Print("target w: ", targetWavelength, ", current: ", playerWave.wavelength);
            GD.Print(Mathf.Abs(playerWave.wavelength - targetWavelength));
            GD.Print("target a:", targetAmplitude, ", current: ", playerWave.amplitude);
            GD.Print(Mathf.Abs(playerWave.wavelength - targetWavelength));
        }

        if (tunedSignal)
        {
            GD.Print("Task complete!");
            // show complete text
            Label wintext = GetNode<Label>("WinText");
            wintext.Visible = true;
            playerWave.waveColor = Colors.Green;
            gameActive = false;

            //newWavelength();
        }

        if (Mathf.Abs(playerWave.wavelength - targetWavelength) < wlTolerance && Mathf.Abs(playerWave.amplitude - targetAmplitude) < ampTolerance)
        {
            alignedTimer += (float)delta;
            if (alignedTimer > 1.0f) tunedSignal = true;
        }
        else alignedTimer = 0.0f;

		if (Input.IsActionJustPressed("close"))
		{
            Close();
		}
	}

    void Close()
    {
		Player plr = GetNode<Player>("../Player");
		plr.setMovementState(MovementStates.FREE_MOVE);
        QueueFree();
    }
}
