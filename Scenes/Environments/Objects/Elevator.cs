using Godot;
using System;

public partial class Elevator : AnimatableBody2D
{
	// The interact boxes that call the elevator to its top and bottom Y positions.
	[Export]
	InteractBox topCallButton;

	[Export]
	InteractBox bottomCallButton;

	// The interact box that causes the elevator to begin moving.
	// Should be a child of this elevator.
	[Export]
	public InteractBox elevatorButton;

	// The collision shapes that make up the walls of this elevator to be toggled when in motion
	// (to prevent the player walking off).
	[Export]
	CollisionShape2D[] wallCollision;

	// The collision shape that makes up the top floor of the building. Will be disabled when the elevator is toggled
	// with the elevatorButton (but NOT the call buttons!)
	[Export]
	CollisionShape2D topFloorCollision;

	// The Y values of the top and bottom floors.
	[Export]
	float topY;

	[Export]
	float bottomY;

	// The time it takes the elevator to travel between floors.
	[Export]
	float travelTime = 3.0f;

	// Whether the elevator is moving or not, and its current (or most recent) direction of travel.
	// Note - inMotion may not be used, remove it if it isn't - Eryth
	private bool inMotion = false;
	private bool goingUp = false;

	// The distance between the top and bottom floors. 
	private float maxDistance;

	// The tween used to animate position
	private Tween tween;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		topCallButton.Interacted += TravelToTop;
		bottomCallButton.Interacted += TravelToBottom;
		elevatorButton.Interacted += ToggleElevator;

		// swap top and bottom floors if the top is below the bottom
		if (topY > bottomY) { (topY, bottomY) = (bottomY, topY); }
		maxDistance = bottomY - topY;

		SetWallsDisabled(true);

		// Enable elevator button when progression updates past waveform
		Globals.Instance.ProgressionChange += () =>
		{
			if (Globals.ProgressionStage > GAMESTAGE.WAVEFORM)
			{
				elevatorButton.active = true;
			}
		};
	}

	private void ToggleElevator()
	{
		topFloorCollision.Disabled = true;
		if (goingUp) { TravelToBottom(); }
		else { TravelToTop(); }
	}

	private void TravelToBottom()
	{
		goingUp = false;
		if (bottomY - Position.Y < 0.5f) return;
		Travel(bottomY);
	}

	private void TravelToTop()
	{
		goingUp = true;
		if (Position.Y - topY < 0.5f) return;
		Travel(topY);
	}

	private void Travel(float destination)
	{
		inMotion = true;
		elevatorButton.active = false;

		if (tween != null) { tween.Kill(); }
		tween = CreateTween();
		tween.SetEase(Tween.EaseType.InOut);
		tween.SetTrans(Tween.TransitionType.Quad);

		// Find out how long the tween should take based on current destination distance compared to maximum distance
		float thisTravelTime = travelTime * (Mathf.Abs(Position.Y - destination) / maxDistance);

		// Create a tween to interpolate between current and destination position
		tween.TweenProperty(this, "position", new Vector2(Position.X, destination), thisTravelTime);
		tween.TweenCallback(Callable.From(FinishTravel));

		SetWallsDisabled(false);
	}

	// called only when the tween used to animate position is completed
	private void FinishTravel()
	{
		inMotion = false;
		if (Globals.ProgressionStage > GAMESTAGE.WAVEFORM || goingUp == false)
		{
			elevatorButton.active = true;
		}
		topFloorCollision.Disabled = false;
		SetWallsDisabled(true);
	}

	private void SetWallsDisabled(bool disabled)
	{
		foreach (CollisionShape2D shape in wallCollision)
		{
			shape.Disabled = disabled;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
