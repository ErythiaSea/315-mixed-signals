using Godot;
using System;
using static Godot.TextServer;

public partial class GravPlayer : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	// possible todo: enum with "player states" instead of millions of bools
	// however im short-sighted and can't figure out what other states i need,
	// which ones need to be mutually exclusive, etc. this is fine for rn :3
	public bool onLadder = false;

	public Vector2 moveTo = Vector2.Zero;

	AnimatedSprite2D playerSprite;

    public override void _Ready()
    {
		FloorConstantSpeed = true;
		FloorSnapLength = 20.0f;

        playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
    }

    public override void _PhysicsProcess(double delta)
	{
		if (moveTo != Vector2.Zero)
		{

		}
		if (!onLadder) standardMovement(delta);
		else ladderMovement(delta);
	}

	void standardMovement(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("print_intersect") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

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
		MoveAndCollide(velocity);
	}
}
