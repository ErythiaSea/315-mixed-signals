using Godot;
using System;

public partial class CameraMovement : Camera2D
{
	[Export]
	public float cameraSpeed = 0.6f;

	private bool canMoveCam = true;
	private ColorRect telescope;
	private Vector2 center;
	private AudioStreamPlayer audioPlayer;

	Globals globalScript;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globalScript = Globals.Instance;
		telescope = GetNode("Telescope") as ColorRect;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(canMoveCam)CameraInputEvents(delta);

		center = GetScreenCenterPosition();
	}

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
	
	private void DisplayConstellation(Vector2 centerStar)
	{
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
	else
	{
		GD.PrintErr("AudioStreamPlayer node not found! Check the node path.");
	}
	
		canMoveCam = false;
		Tween completetion = GetTree().CreateTween();
	   
		completetion.Parallel().TweenProperty(this, "zoom", new Vector2(0.4f, 0.4f), 6f); //needs to be at least 5.6 seconds for reveal audio
		completetion.Parallel().TweenProperty(this, "position", centerStar, 1f);
		completetion.Parallel().TweenProperty(telescope, "scale", new Vector2(2.5f,2.5f), 6f); //needs to be at least 5.6 seconds for reveal audio
		completetion.TweenCallback(Callable.From(this.UpdateGlobals));
	}

	private void UpdateGlobals()
	{
		globalScript.gameState.stage = GAMESTAGE.END;
	}
}
