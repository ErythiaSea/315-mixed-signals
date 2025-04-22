using Godot;
using System;
using static Godot.TextServer;

// note: NONE is used for testing to see if none are set and cannot be used with bitwise ops!!
[Flags]
public enum MovementStates { NONE = 0, FREE_MOVE = 1, LADDER_MOVE = 2 };

public partial class Player : CharacterBody2D
{

    [Signal]
    public delegate void TransitionEventHandler(TRANSITION type, float transitionLength);
    
	[Export]
	public float movementSpeed = 600.0f;

	[Export]
	PackedScene walkingParticles;

	MovementStates playerMovementState = MovementStates.FREE_MOVE;
	bool isMovementLocked = false;

	// for moving the player in cutscenes (or centering on ladder)
	public bool isAutoWalking = false; 
	public float autoWalkDestinationX = float.MinValue;

	private int lastStep = 0;
	AnimatedSprite2D playerSprite;
	Sprite2D interactSprite;
	Area2D interactArea;
	Camera2D playerCamera;
	Level thisLevel;

    public override void _Ready()
    {
		FloorConstantSpeed = true;
		FloorSnapLength = 20.0f;
		FloorMaxAngle = Mathf.DegToRad(66);

        playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		interactSprite = GetNode<Sprite2D>("InteractSprite");
		interactArea = GetNode<Area2D>("InteractArea");
        playerCamera = GetNode<Camera2D>("PlayerCamera");
		thisLevel = GetParent<Level>();
        SignalBus.Instance.DialogueEnded += OnDialogueEnded;

		// in any scene that contains the player, overworld should be the base gamestate
		Globals.SetGamestate(GAMESTATE.OVERWORLD);
    }

    public override void _Process(double delta)
    {

		if (Globals.Gamestate != GAMESTATE.OVERWORLD || isMovementLocked) return;

        interactSprite.Visible = false;
        foreach (Area2D area in interactArea.GetOverlappingAreas())
        {
            InteractBox interactBox = area as InteractBox;
            if (interactBox != null && interactBox.active)
            {
                interactSprite.Visible = true;
				interactBox.isPlayerInArea = true;
                if ((interactBox.isAutofire || (Input.IsActionJustPressed("interact")) && !isMovementLocked))
                {
                    interactBox.Interact(this);
                }
            }
        }

        // Animate player sprite based on the velocity of the player
        if (Velocity.X != 0.0f)
        {
            playerSprite.Play("run");
            playerSprite.FlipH = Velocity.X > 0;

			if(playerSprite.Animation == "run")
			{
				if (playerSprite.Frame == 1 ||  playerSprite.Frame == 4)
				{
                    if (lastStep != playerSprite.Frame)
                    {
						lastStep = playerSprite.Frame;

						// skip trying to emit particles if there are none assigned
						if (walkingParticles == null)
						{
							return;
						}
                        GD.Print("EMITTING PARTICLES");
                        GpuParticles2D particles = walkingParticles.Instantiate() as GpuParticles2D;
                        particles.Emitting = true;
                        particles.GlobalPosition = new Vector2(GlobalPosition.X + (Mathf.Clamp(Velocity.X,-10,10)), GlobalPosition.Y + 200);

						ParticleProcessMaterial mat = particles.ProcessMaterial as ParticleProcessMaterial;
						mat.Direction = new Vector3(-Mathf.Clamp(Velocity.X,-1,1) , mat.Direction.Y,0);
                        GetParent().AddChild(particles);
                    }
				}
			}
			else
			{
				lastStep = -1;
			}
        }
        else
        {
			playerSprite.Play("idle");
        }
    }

    public override void _PhysicsProcess(double delta)
	{
		if (isAutoWalking) { AutoMovement(delta); return; }

		if (isMovementLocked) {
			playerSprite.Play("idle");
			return;
		}
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
	}

	public void SetSpriteFlipH(bool flipH)
	{
		playerSprite.FlipH = flipH;
	}

	public void SetCameraLimits(int left, int top, int right, int bottom)
	{
		// if all limits are the same, the camera should be disabled
		// in theory this should never be true, level script should prevent this
		if (left == right && left == top && left == bottom)
		{
			playerCamera.Enabled = false;
			return;
		}

		playerCamera.LimitBottom = bottom;
		playerCamera.LimitLeft = left;
		playerCamera.LimitTop = top;
		playerCamera.LimitRight = right;
	}

    // camera can be disabled by anyone, but requires the level to have an active camera to be enabled
    public void SetCameraEnabled(bool enabled)
	{
		if (!enabled || thisLevel.getCameraEnabled())
		{
			playerCamera.Enabled = enabled;
		}
	}

	public void SetCameraZoom(float zoom)
	{
		playerCamera.Zoom = new Vector2(zoom, zoom);
	}
	public void setCameraOffsets(float x,float y)
	{
		playerCamera.Offset = new Vector2(x, y);
	}
	void OnDialogueEnded()
	{
		GD.Print("dialogue");
		isMovementLocked = false;
	}

	void OnMinigameClosed()
	{

	}
}

