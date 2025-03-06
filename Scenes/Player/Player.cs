using Godot;
using System;
using static Godot.TextServer;

// note: NONE is used for testing to see if none are set and cannot be used with bitwise ops!!
[Flags]
public enum MovementStates { NONE = 0, FREE_MOVE = 1, LADDER_MOVE = 2 };

public partial class Player : CharacterBody2D
{
	// todo: these are godot defaults. maybe change/export these?
	[Export]
	public float movementSpeed = 600.0f;
	//public const float JumpVelocity = -400.0f;

	MovementStates playerMovementState = MovementStates.FREE_MOVE;
	bool isMovementLocked = false;

	// for moving the player in cutscenes (or centering on ladder)
	public bool isAutoWalking = false; 
	public float autoWalkDestinationX = float.MinValue;

	//public Vector2 moveTo = Vector2.Zero;

	AnimatedSprite2D playerSprite;
	Sprite2D interactSprite;
	Area2D interactArea;

    public override void _Ready()
    {
		FloorConstantSpeed = true;
		FloorSnapLength = 20.0f;
		FloorMaxAngle = Mathf.DegToRad(60);

        playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		interactSprite = GetNode<Sprite2D>("InteractSprite");
		interactArea = GetNode<Area2D>("InteractArea");

		Globals.Instance.DialogueClosed += OnDialogueClosedEvent;

		// spawn point stuff if one is set
		if (Globals.Instance.currentSpawnPoint >= 0) {
			Node2D spawnLocation = GetNode<Node2D>("../SpawnPoints/" + Globals.Instance.currentSpawnPoint);
			Position = spawnLocation.Position;

			// clear spawn point info
            Globals.Instance.currentSpawnPoint = -1;
        }
        playerSprite.FlipH = !(Globals.Instance.spawnFacingLeft);
    }

    public override void _Process(double delta)
    {
        interactSprite.Visible = false;
        foreach (Area2D area in interactArea.GetOverlappingAreas())
        {
            InteractBox interactBox = area as InteractBox;
            if (interactBox != null && interactBox.active)
            {
                interactSprite.Visible = true;
                if (interactBox.isAutofire || (Input.IsActionJustPressed("interact") && !isMovementLocked))
                {
                    interactBox.Interact(this);
					//setMovementState(MovementStates.MOVE_LOCKED);
                    //playerMovementState = MovementStates.MOVE_LOCKED;
                }
            }
        }

        // Animate player sprite based on the velocity of the player
        if (Velocity.X != 0.0f)
        {
            playerSprite.Play();
            playerSprite.FlipH = Velocity.X > 0;
        }
        else
        {
            playerSprite.Frame = 0;
            playerSprite.Pause();
        }
    }

    public override void _PhysicsProcess(double delta)
	{
		if (isAutoWalking) { AutoMovement(delta); return; }

		if (isMovementLocked) { return; }
		if (playerMovementState == MovementStates.FREE_MOVE) StandardMovement(delta);
		else if (playerMovementState == MovementStates.LADDER_MOVE) LadderMovement(delta);
	}

	private void StandardMovement(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

        // Handle Jump.
        //if (Input.IsActionJustPressed("interact") && IsOnFloor())
        //{
        //	velocity.Y = JumpVelocity;
        //}

        // Get the input direction and handle the movement/deceleration.
        //Vector2 direction = Input.GetVector("left_pivot_cw", "left_pivot_ccw", "up", "down");
        float xDirection = Input.GetAxis("left_pivot_cw", "left_pivot_ccw");
		if (xDirection != 0)
		{
			velocity.X = xDirection * movementSpeed;
			if (Input.IsActionPressed("middle_mouse")) velocity.X *= 2;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, movementSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void LadderMovement(double delta)
	{
		GD.Print("trying ladder move");
		Velocity = Vector2.Zero;
		Vector2 velocity = Vector2.Zero;
		float yDirection = Input.GetAxis("up", "down");
		if (yDirection != 0)
		{
			velocity.Y = yDirection * movementSpeed;
		}
		else
		{
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, movementSpeed);
		}

		//Velocity = velocity;
		MoveAndCollide(velocity*(float)delta);
	}

	private void AutoMovement(double delta)
	{
		// todo: eliminate redundant calculations? with a memory cost instead?
		float dir = (Position.X - autoWalkDestinationX) > 0 ? -1 : 1;
		Velocity = new Vector2(movementSpeed*dir, 0);
		MoveAndSlide();
		if (Mathf.Abs(Position.X - autoWalkDestinationX) < 5.0f) { GD.Print("done"); isAutoWalking = false; }
	}
	public void ToggleLadder()
	{
		if (playerMovementState == MovementStates.FREE_MOVE) SetMovementState(MovementStates.LADDER_MOVE);
        else if (playerMovementState == MovementStates.LADDER_MOVE) SetMovementState(MovementStates.FREE_MOVE);
    }

	public void SetMovementState(MovementStates state)
	{
        // 2 = world, 4 = ladderbox
        if (state == MovementStates.FREE_MOVE)
        {
            SetCollisionMaskValue(2, true);
            SetCollisionMaskValue(4, false);
        }
        if (state == MovementStates.LADDER_MOVE)
		{
			SetCollisionMaskValue(2, false);
            SetCollisionMaskValue(4, true);
        }
		playerMovementState = state;
	}

	public void SetMovementLock(bool locked) 
	{ 
		isMovementLocked = locked;
		if (!locked)
		{
            // enable player cam if we have one
            Camera2D plrCam = GetNode<Camera2D>("Camera2D");
            if (plrCam != null) { plrCam.Enabled = true; }
        }
	}

	//public void FlipSprite()

	public void OnDialogueClosedEvent()
	{
		isMovementLocked = false;
	}
}

