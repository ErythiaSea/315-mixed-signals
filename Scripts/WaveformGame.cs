using Godot;
using System;

public partial class WaveformGame : Node2D
{
	WaveRender playerWave, realWave;
    float targetWavelength, targetAmplitude;

    float tolerance = 2.0f;
    float alignedTimer = 0.0f;
    bool tunedSignal = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerWave = GetNode<WaveRender>("playerwave");
        realWave = GetNode<WaveRender>("realwave");
        newWavelength();
    }

    void newWavelength()
    {
        targetWavelength = (GD.Randf() * 200.0f) + 10.0f;
        realWave.wavelength = targetWavelength;

        targetAmplitude = (GD.Randf() * 150.0f) + 100.0f;
        realWave.amplitude = targetAmplitude;
        tunedSignal = false; alignedTimer = 0.0f;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        float wlChange = 0.5f;
        float wlMult = (playerWave.wavelength / 100.0f);
        if (Input.IsActionPressed("left_pivot_cw"))
        {
            playerWave.amplitude += 0.5f;
        }
        if (Input.IsActionPressed("left_pivot_ccw"))
        {
            playerWave.amplitude -= 0.5f;
        }
        if (Input.IsActionPressed("right_pivot_cw"))
        {
            playerWave.wavelength += wlChange * wlMult;
            //playerWave.frequency += wlChange;
        }
        if (Input.IsActionPressed("right_pivot_ccw"))
        {
            playerWave.wavelength -= wlChange * wlMult;
            //playerWave.frequency -= wlChange;
        }

        if (playerWave.amplitude < 0.0f) playerWave.amplitude = 0.0f;
        if (Input.IsActionPressed("print_intersect"))
        {
            GD.Print("target w: ", targetWavelength, ", current: ", playerWave.wavelength);
            GD.Print(Mathf.Abs(playerWave.wavelength - targetWavelength));
            GD.Print("amp:", playerWave.amplitude);
        }

        if (tunedSignal)
        {
            GD.Print("Task complete!");
            // show complete text
            Label wintext = GetNode<Label>("WinText");
            wintext.Visible = true;
            newWavelength();
        }
        if (Mathf.Abs(playerWave.wavelength - targetWavelength) < tolerance && Mathf.Abs(playerWave.amplitude - targetAmplitude) < tolerance)
        {
            alignedTimer += (float)delta;
            if (alignedTimer > 1.0f) tunedSignal = true;
        }
        else alignedTimer = 0.0f;

		if (Input.IsActionJustPressed("close"))
		{
            close();
		}
	}

    void close()
    {
        Player plr = GetNode<Player>("../Player");
        plr.canMove = true;
        GD.Print("we freeing");
        QueueFree();
    }
}
