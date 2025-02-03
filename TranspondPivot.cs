using Godot;
using System;

public partial class TranspondPivot : Node2D
{
	[Export]
	bool isLeft = false;
    Transpond transpond;

    StringName cwInput, ccwInput;
	public Area2D area;
	public Sprite2D sprite;

	public bool overlapsTower = false;

	float rotSpeed = 0.75f; // in rad/s

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		area = GetNode<Area2D>("lineArea");
		sprite = GetNode<Sprite2D>("lineSprite");

		transpond = GetNode<Transpond>(".."); // glue

		if (isLeft) {
			cwInput = "left_pivot_cw";
			ccwInput = "left_pivot_ccw";
		} else {
            cwInput = "right_pivot_cw";
            ccwInput = "right_pivot_ccw";
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed(ccwInput))
		{
			Rotate(rotSpeed * (float)delta);
		}
        if (Input.IsActionPressed(cwInput))
        {
            Rotate(-rotSpeed * (float)delta);
        }


        if (area.OverlapsArea(transpond.currentTower))
		{
			sprite.Modulate = new Color(0, 1, 0, 1);
			overlapsTower = true;
		}
		else
		{
			sprite.Modulate = new Color(1, 0, 0, 1);
			overlapsTower = false;
		}
	}
}
