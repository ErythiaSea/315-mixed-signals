using Godot;
using System;

public partial class RadiotowerPivot : Node2D
{
	[Export]
	bool isLeft = false;
    Radiotower radiotower;

    StringName cwInput, ccwInput;
	public Area2D area;
	public Sprite2D sprite;
	public AudioStreamPlayer2D streamPlayer;
	AudioEffectDistortion distortEffect;

	public bool overlapsTower = false;

	[Export] float maxRotationSpeed = 0.75f; // in rad/s
	[Export] float rotationAccel = 0.08f;
	[Export] float rotationFriction = 0.16f;
	float rotSpeed = 0.0f;
	float maxRotDeg, minRotDeg;

	public bool handleInputs = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		area = GetNode<Area2D>("lineArea");
		sprite = GetNode<Sprite2D>("lineSprite");

		radiotower = GetNode<Radiotower>(".."); // glue
		streamPlayer = GetNode<AudioStreamPlayer2D>("lineAudio");

		if (isLeft) {
			cwInput = "left_pivot_cw";
			ccwInput = "left_pivot_ccw";
			minRotDeg = 270; maxRotDeg = 360;
			

        } else {
            cwInput = "right_pivot_cw";
            ccwInput = "right_pivot_ccw";
            minRotDeg = 180; maxRotDeg = 270;
            distortEffect = (AudioEffectDistortion)AudioServer.GetBusEffect(2, 0);
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (!handleInputs) return;

        // Rotate pivots
        float dir = Input.GetAxis(cwInput, ccwInput);
		if (dir != 0)
		{
			rotSpeed = Mathf.MoveToward(rotSpeed, dir * maxRotationSpeed, rotationAccel);
		}
		else {
            rotSpeed = Mathf.MoveToward(rotSpeed, 0, rotationFriction);
        }
        Rotate(rotSpeed*(float)delta);

		// Reset rotation speed if we hit a "wall"
		float ogRotDeg = RotationDegrees;
		RotationDegrees = Mathf.Clamp(RotationDegrees, minRotDeg, maxRotDeg);
		if (RotationDegrees != ogRotDeg) rotSpeed = 0;

        if (area.OverlapsArea(radiotower.currentTower))
		{
			//sprite.Modulate = new Color(0, 1, 0, 1);
			//streamPlayer.PitchScale = 3;
			//distortEffect.Drive = 0;
			overlapsTower = true;
		}
		else
		{
			sprite.Modulate = new Color(1, 0, 0, 1);
			//streamPlayer.PitchScale = 1;
			//distortEffect.Drive = 0.67f;
			overlapsTower = false;
		}
	}
}
