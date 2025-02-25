using Godot;
using System;
using static Godot.TextServer;

// note: NONE is used for testing to see if none are set and cannot be used with bitwise ops!!
[Flags]
public enum MovementStates { NONE = 0, FREE_MOVE = 1, LADDER_MOVE = 2, MOVE_LOCKED = 4 };

public partial class GravPlayer : CharacterBody2D
{
	// todo: these are godot defaults. maybe change/export these?
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	// note: im assigning the playerMovementState to one of these states. however, you
	// could use this as flags with bitwise ops to have, say, LADDER_MOVE and MOVE_LOCKED
	// both be true, so when you unlock movement you return to ladder movement
	MovementStates playerMovementState = MovementStates.FREE_MOVE;

	// for moving the player in cutscenes (or centering on ladder)
	public bool autoWalk = false; 
	public float autoWalkDestinationX = float.MinValue;

	//public Vector2 moveTo = Vector2.Zero;

	AnimatedSprite2D playerSprite;
	Sprite2D interactSprite;
	Area2D interactArea;

    public override void _Ready()
    {
		FloorConstantSpeed = true;
		FloorSnapLength = 20.0f;

        playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		interactSprite = GetNode<Sprite2D>("InteractSprite");
		interactArea = GetNode<Area2D>("InteractArea");
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
                if (Input.IsActionJustPressed("print_intersect") && playerMovementState != MovementStates.MOVE_LOCKED)
                {
                    interactBox.Interact(this);
                    //playerMovementState = MovementStates.MOVE_LOCKED;
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
	{
		if (autoWalk) { autoMovement(delta); return; }

		if (playerMovementState == MovementStates.FREE_MOVE) standardMovement(delta);
		else if (playerMovementState == MovementStates.LADDER_MOVE) ladderMovement(delta);
	}

	private void standardMovement(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		//if (Input.IsActionJustPressed("print_intersect") && IsOnFloor())
		//{
		//	velocity.Y = JumpVelocity;
		//}

		// Get the input direction and handle the movement/deceleration.
		//Vector2 direction = Input.GetVector("left_pivot_cw", "left_pivot_ccw", "up", "down");
		float xDirection = Input.GetAxis("left_pivot_cw", "left_pivot_ccw");
		if (xDirection != 0)
		{
			velocity.X = xDirection * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;

		// Animate player sprite based on the velocity of the player
		if (Velocity != Vector2.Zero)
		{
			playerSprite.Play();
			if (Velocity.X != 0) playerSprite.FlipH = Velocity.X > 0;
		}
		else
		{
			playerSprite.Frame = 0;
			playerSprite.Pause();
		}
		MoveAndSlide();
	}

	private void ladderMovement(double delta)
	{
		Vector2 velocity = Vector2.Zero;
		float yDirection = Input.GetAxis("up", "down");
		if (yDirection != 0)
		{
			velocity.Y = yDirection * Speed;
		}
		else
		{
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		//Velocity = velocity;
		MoveAndCollide(velocity*(float)delta);
	}

	private void autoMovement(double delta)
	{
		// todo: eliminate redundant calculations? with a memory cost instead?
		float dir = (Position.X - autoWalkDestinationX) < 0 ? -1 : 1;
		Velocity = new Vector2(Speed, 0);
		MoveAndSlide();
		if (Mathf.Abs(Position.X - autoWalkDestinationX) < 3.0f) autoWalk = false;
	}

	public void toggleLadder()
	{
		if (playerMovementState == MovementStates.FREE_MOVE) setMovementState(MovementStates.LADDER_MOVE);
        else if (playerMovementState == MovementStates.LADDER_MOVE) setMovementState(MovementStates.FREE_MOVE);
    }

	public void setMovementState(MovementStates state)
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
}

