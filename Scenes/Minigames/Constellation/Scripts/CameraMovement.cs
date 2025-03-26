using Godot;
using System;

public partial class CameraMovement : Camera2D
{
	[Export]
	public float cameraSpeed = 0.6f;

	public bool canMoveCam = true;
	private ColorRect telescope;
	private Vector2 center;

	Globals globalScript;
	
	// This is the easy way out but this really needs a whole redesign - Eryth
	ConstellationMinigame minigame;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globalScript = Globals.Instance;
		telescope = GetNode("Telescope") as ColorRect; //GetNode<ColorRect>("Telescope") ???
		minigame = GetParent<ConstellationMinigame>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(canMoveCam)CameraInputEvents(delta);

		center = GetScreenCenterPosition();
	}
	
	// todo: make relative to deltatime
	public void CameraInputEvents(double delta)
	{
		if (Input.IsActionPressed("down"))
		{
			Position = new Vector2(Position.X, Position.Y + cameraSpeed);
		}
		if (Input.IsActionPressed("up"))
		{
			Position = new Vector2(Position.X, Position.Y - cameraSpeed);
		}
		if (Input.IsActionPressed("left"))
		{
			Position = new Vector2(Position.X - cameraSpeed, Position.Y);
		}
		if (Input.IsActionPressed("right"))
		{
			Position = new Vector2(Position.X + cameraSpeed, Position.Y);
		}
	}

	public void DisplayConstellation(Vector2 centerStar)
	{
		canMoveCam = false;
		Tween completion = GetTree().CreateTween();

		completion.Parallel().TweenProperty(this, "zoom", new Vector2(0.4f, 0.4f), 2f);
		completion.Parallel().TweenProperty(this, "position", centerStar, 1f);
		completion.Parallel().TweenProperty(telescope, "scale", new Vector2(2.5f, 2.5f), 2f);
		completion.TweenInterval(1.5);
		completion.TweenCallback(Callable.From(minigame.ShowFinalBox));

		// Get the AudioStreamPlayer node (adjust the path if needed)
		AudioStreamPlayer audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		// Load the "Reveal" sound and play it
		if (audioPlayer != null)
		{
			GD.Print("AudioStreamPlayer found!");

			var revealSound = (AudioStream)GD.Load("res://Scenes/Minigames/Constellation/Scripts/Reveal.wav");
			if (revealSound != null)
			{
				audioPlayer.Stream = revealSound;
				audioPlayer.VolumeDb = 0; // Ensure it's audible
				audioPlayer.Play();
				GD.Print("Playing Reveal sound...");
			}
			else
			{
				GD.PrintErr("Failed to load Reveal sound. Check the file path.");
			}
		}
	}
}
