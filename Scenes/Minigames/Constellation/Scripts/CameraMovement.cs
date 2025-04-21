using Godot;
using System;

public partial class CameraMovement : Camera2D
{
	[Export]
	public float cameraSpeed = 0.6f;

	public bool canMoveCam = true;

	private ColorRect telescope;
	private Vector2 center;

	AudioStreamPlayer audioStreamPlayer;
	
	// This is the easy way out but this really needs a whole redesign - Eryth
	ConstellationMinigame minigame;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		telescope = GetNode("Telescope") as ColorRect; //GetNode<ColorRect>("Telescope") ???
		minigame = GetParent<ConstellationMinigame>();
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (canMoveCam && Globals.Gamestate == GAMESTATE.CONSTELLATION)
		{
			CameraInputEvents(delta);
		}

		center = GetScreenCenterPosition();
	}
	
	// todo: make relative to deltatime
	public void CameraInputEvents(double delta)
	{

		//Moves camera based on input keys mapped in project settings
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

	//Zooms out and shows the completed constellation, centered on the star with the most connections
	public void DisplayConstellation(Vector2 centerStar)
	{
		GD.Print("displaying constellation...");
		canMoveCam = false;
		Tween completion = GetTree().CreateTween();

		completion.Parallel().TweenProperty(this, "zoom", new Vector2(0.35f, 0.35f), 2f);
		completion.Parallel().TweenProperty(this, "position", centerStar, 1f);
		completion.Parallel().TweenProperty(telescope, "scale", new Vector2(2.5f, 2.5f), 2f);
		completion.TweenInterval(1.5);
		completion.TweenCallback(Callable.From(minigame.ShowFinalBox));

		audioStreamPlayer.Play();
	}
}
