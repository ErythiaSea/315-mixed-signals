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
	//[Export]
	//public InteractBox elevatorButton;

	// The Area2D that will make the elevator move when the player reaches it.
	[Export]
	public Area2D elevatorCenter;

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

	// If the elevator button was pressed, this is true until the player reaches the elevator
	private bool readyToMove = false;

	// The distance between the top and bottom floors. 
	private float maxDistance;

	// The tween used to animate position
	private Tween tween;

	// i would rather not have to do this but too late
	Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		topCallButton.Interacted += TravelToTop;
		bottomCallButton.Interacted += TravelToBottom;
		//elevatorButton.Interacted += ToggleElevator;
		elevatorCenter.AreaEntered += (Area2D area) => {
			if (area.GetParent() is Player)
			{
				ToggleElevator();
			}
		};

		// swap top and bottom floors if the top is below the bottom
		if (topY > bottomY) { (topY, bottomY) = (bottomY, topY); }
		maxDistance = bottomY - topY;

		SetWallsDisabled(true);

		// Enable elevator button when progression updates past waveform
		//Globals.Instance.ProgressionChange += () =>
		//{
		//	if (Globals.ProgressionStage > GAMESTAGE.WAVEFORM)
		//	{
		//		bottomCallButton.active = true;
		//	}
		//};

		player = GetNode<Player>("../Player");
	}

	private void ToggleElevator()
	{
		if (!readyToMove) return;
		readyToMove = false;
		topFloorCollision.SetDeferred("disabled", true);
		if (goingUp) { TravelToBottom(); }
		else { TravelToTop(); }
	}

	private void TravelToBottom()
	{
		goingUp = false;
		
		// move the player up 
		if (bottomY - Position.Y < 0.5f)
		{
			readyToMove = true;
			player.autoWalkDestinationX = Position.X;
			player.isAutoWalking = true;
			return;
		}
		// bring the elevator down
		else
		{
			Travel(bottomY);
		}
	}

	private void TravelToTop()
	{
		goingUp = true;
		// move the player down
		if (Position.Y - topY < 0.5f)
		{
			readyToMove = true;
			player.autoWalkDestinationX = Position.X;
			player.isAutoWalking = true;
			return;
		}
		// bring the elevator up
		else
		{
			Travel(topY);
		}
	}

	private void Travel(float destination)
	{
		inMotion = true;
		//elevatorButton.active = false;
		bottomCallButton.active = false; topCallButton.active = false;

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
		//if (Globals.ProgressionStage > GAMESTAGE.WAVEFORM || goingUp == false)
		//{
		bottomCallButton.active = true; topCallButton.active = true;
		//}
		topFloorCollision.SetDeferred("disabled", false);
		SetWallsDisabled(true);
	}

	private void SetWallsDisabled(bool disabled)
	{
		foreach (CollisionShape2D shape in wallCollision)
		{
			shape.SetDeferred("disabled", disabled);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
