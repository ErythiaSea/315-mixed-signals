using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class WaveRender : Node2D
{
	[Export] // Wave amplitude (how tall it is)
    float amplitude = 250.0f;
	[Export] // The length of the entire drawn wave shape
	float shapelength = 500.0f;
	[Export] // Length of each individual wave
	float wavelength = 100.0f;
	[Export] // Wave frequency (how fast it moves)
	float frequency = 100.0f;
	[Export] // Number of points in array (higher = better looking wave, more expensive)
	int numPoints = 300;

	[Export]
	float randFactor = 0.0f;

    [Export]
    Color waveColor;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionPressed("left_pivot_cw"))
        {
			randFactor += 0.01f;
        }
        if (Input.IsActionPressed("left_pivot_ccw"))
        {
            randFactor -= 0.01f;
        }
        if (Input.IsActionPressed("right_pivot_cw"))
        {
			wavelength += 1;
        }
        if (Input.IsActionPressed("right_pivot_ccw"))
        {
			wavelength -= 1;
			GD.Print(wavelength);
        }
        if (Input.IsActionPressed("print_intersect"))
        {
			randFactor = 0.0f;
        }
        QueueRedraw();
	}

    public override void _Draw()
    {
		Vector2[] points = new Vector2[numPoints];
		float dist = shapelength / numPoints;
		float t = (float)Time.GetTicksMsec()/1000.0f; 

        for (int i = 0; i < numPoints; i++)
		{
			float x = i*dist;
			float y = Mathf.Sin((x/wavelength) + t * frequency) * amplitude;
			y += (GD.Randf()-0.5f) * amplitude * randFactor;
			y = Mathf.Clamp(y, -amplitude*1.5f, amplitude*1.5f);
			points[i] = (new Vector2(x, y));
		}

		DrawPolyline(points, waveColor);
    }
}
