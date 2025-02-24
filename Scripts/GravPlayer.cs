using Godot;
using System;

public partial class GravPlayer : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	AnimatedSprite2D playerSprite;

    public override void _Ready()
    {
		FloorConstantSpeed = true;
		FloorSnapLength = 20.0f;

        playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
    }

    public override void _PhysicsProcess(double delta)
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
		// As good practice, you should replace UI actions with custom gameplay actions.

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
}
