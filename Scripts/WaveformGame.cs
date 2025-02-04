using Godot;
using System;

public partial class WaveformGame : Node2D
{
	WaveRender playerWave, realWave;
    float targetWavelength;

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
        tunedSignal = false; alignedTimer = 0.0f;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionPressed("left_pivot_cw"))
        {
            playerWave.amplitude += 0.01f;
        }
        if (Input.IsActionPressed("left_pivot_ccw"))
        {
            playerWave.amplitude -= 0.01f;
            newWavelength();
        }
        if (Input.IsActionPressed("right_pivot_cw"))
        {
            playerWave.wavelength += 0.5f;
        }
        if (Input.IsActionPressed("right_pivot_ccw"))
        {
            playerWave.wavelength -= 0.5f;
            //GD.Print(playerWave.wavelength);
        }
        if (Input.IsActionPressed("print_intersect"))
        {
            GD.Print("target w: ", targetWavelength, ", current: ", playerWave.wavelength);
            GD.Print(Mathf.Abs(playerWave.wavelength - targetWavelength));
        }

        if (tunedSignal)
        {
            GD.Print("Task complete!");
            newWavelength();
        }
        if (Mathf.Abs(playerWave.wavelength - targetWavelength) < 2.0f)
        {
            alignedTimer += (float)delta;
            if (alignedTimer > 1.0f) tunedSignal = true;
        }
        else alignedTimer = 0.0f;
    }
}
