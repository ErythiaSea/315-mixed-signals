using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class WaveRender : Node2D
{
	// note to self: do not have all of these public :bangbang:

	[Export] // Wave amplitude (how tall it is)
    public float amplitude = 250.0f;
	[Export] // The length of the entire drawn wave shape
    public float shapelength = 500.0f;
	[Export] // Length of each individual wave
    public float wavelength = 100.0f;
	[Export] // Wave frequency (how fast it moves)
    public float frequency = 100.0f;
	[Export] // If the frequency is actually the number of pixels moved by the wave per second (i.e. not relative to wavelength)
	public bool frequencyAsSpeed = true;
	[Export] // Number of points in array (higher = better looking wave, more expensive)
    public int numPoints = 300;

	[Export] // some "static" (au comm sab)
	public float randFactor = 0.0f;
    [Export] // color of the drawn line
    public Color waveColor;
	[Export] // thickness of the drawn line
	float lineThick = 1.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        QueueRedraw();
	}

    public override void _Draw()
    {
		Vector2[] points = new Vector2[numPoints];
		float dist = shapelength / numPoints; // distance between each point on x axis
		float t = (float)Time.GetTicksMsec()/1000.0f;
		t = t + 100.0f;

        for (int i = 0; i < numPoints; i++)
		{
			float x = i*dist - (shapelength/2);
			float freq = frequency / (frequencyAsSpeed ? wavelength : 1.0f);
			float y = Mathf.Sin(((x/wavelength) * freq) + t) * amplitude;
			y += (GD.Randf()-0.5f) * amplitude * randFactor;
			y = Mathf.Clamp(y, -amplitude*1.5f, amplitude*1.5f);
			points[i] = (new Vector2(x, y));
		}

		DrawPolyline(points, waveColor, lineThick);
    }
}
