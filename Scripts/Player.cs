using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] // how much the player moves up when holding up on not-ladders
	float yMovementFactor = 1.0f;

	[Export]
	float moveSpeed = 250.0f;

    Sprite2D interactSprite;
    Area2D interactArea;
    AnimatedSprite2D playerSprite;

    public bool restrictHorizontal = false, restrictVertical = false;
    public bool canMove = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        interactArea = GetNode<Area2D>("InteractArea");
        interactSprite = GetNode<Sprite2D>("InteractSprite");
        playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Velocity = Vector2.Zero;

        if (canMove) HandleInput();

        interactSprite.Visible = false;
        if (interactArea.HasOverlappingAreas()) {
            foreach (Area2D area in interactArea.GetOverlappingAreas()) {
                InteractBox interactBox = area as InteractBox;
                if (interactBox != null)
                {
                    interactSprite.Visible = true;
                    if (Input.IsActionJustPressed("print_intersect") && canMove)
                    {
                        interactBox.Interact(this);
                        canMove = false;
                    }
                }
            }
        }

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

    void HandleInput()
    {
        if (!restrictHorizontal)
        {
            if (Input.IsActionPressed("left_pivot_ccw"))
            {
                Velocity = Velocity + new Vector2(moveSpeed, 0);
            }
            if (Input.IsActionPressed("left_pivot_cw"))
            {
                Velocity = Velocity + new Vector2(-moveSpeed, 0);
            }
        }

        if (!restrictVertical)
        {
            if (Input.IsActionPressed("up"))
            {
                Velocity = Velocity + new Vector2(0, -moveSpeed * yMovementFactor);
            }
            if (Input.IsActionPressed("down"))
            {
                Velocity = Velocity + new Vector2(0, moveSpeed * yMovementFactor);
            }
        }
    }
}
