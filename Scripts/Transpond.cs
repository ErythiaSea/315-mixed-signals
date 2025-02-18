using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class Transpond : Node2D
{
	Sprite2D intersectIndicator;
	TranspondPivot lPivot, rPivot;

	Sprite2D leftIndicator, rightIndicator;
	float leftTimer, leftInterval, rightTimer, rightInterval;

    Godot.Collections.Array<Node> towers;
    public Node2D currentTower;
	int idx = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		intersectIndicator = GetNode<Sprite2D>("intersectIndicator");
        leftIndicator = GetNode<Sprite2D>("leftIndicator");
        rightIndicator = GetNode<Sprite2D>("rightIndicator");
        lPivot = GetNode<TranspondPivot>("leftPivot");
        rPivot = GetNode<TranspondPivot>("rightPivot");

        towers = GetNode("towers").GetChildren();
        idx = (int)(GD.Randi() % 7);
		currentTower = (Node2D)towers[idx];
        GD.Print("len towers is: ", towers.Count);

		leftInterval = 0.75f; rightInterval = 0.75f;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 intersect = Vector2.Zero;
		if (lPivot.Rotation == rPivot.Rotation) intersectIndicator.Visible = false; // lines parallel
        else
		{
			intersectIndicator.Visible = true;
			//intersect = CalcIntersect();
			//intersectIndicator.Position = intersect;
		}

		// get distance from pivot to tower for indicators
		Vector2 closestPointOnLine = pointOnPivot(lPivot, currentTower.Position);
		float dist = closestPointOnLine.DistanceTo(currentTower.Position);
		leftInterval = ((Mathf.Min(200.0f, dist)+20.0f) / 220.0f) * 0.75f;

        closestPointOnLine = pointOnPivot(rPivot, currentTower.Position);
        dist = closestPointOnLine.DistanceTo(currentTower.Position);
        rightInterval = ((Mathf.Min(200.0f, dist) + 20.0f) / 220.0f) * 0.75f;

		// handle indicator stuff
        leftTimer += (float)delta; rightTimer += (float)delta;
		if (leftTimer > leftInterval) {
			leftIndicator.Visible = !leftIndicator.Visible;
			leftTimer = 0.0f;
		}
        if (rightTimer > rightInterval) {
            rightIndicator.Visible = !rightIndicator.Visible;
            rightTimer = 0.0f;
        }

        if (Input.IsActionJustPressed("print_intersect"))
		{
			//GD.Print("Lines intersect at coords:" + intersect);
			GD.Print(dist);
			if (lPivot.overlapsTower && rPivot.overlapsTower)
			{
				GD.Print("Tower found!");
				int ogIdx = idx;
				do { idx = (int)(GD.Randi() % 7); } while (idx == ogIdx);
				GD.Print(idx);
				currentTower = (Node2D)towers[idx];

				// show complete text
				Label wintext = GetNode<Label>("WinText");
				wintext.Visible = true;
			}
		}

        if (Input.IsActionJustPressed("close"))
		{
			Player plr = GetNode<Player>("../Player");
			plr.canMove = true;
			QueueFree();
		}

    }

	public Vector2 CalcIntersect()
	{
		Vector2 lDirection = Vector2.Right.Rotated(lPivot.Rotation);
        Vector2 rDirection = Vector2.Right.Rotated(rPivot.Rotation);
        return (Vector2)Geometry2D.LineIntersectsLine(lPivot.Position, lDirection, rPivot.Position, rDirection);
	}

	Vector2 pointOnPivot(Node2D pivot, Vector2 pointPos)
	{
        Vector2 dir = Vector2.Right.Rotated(pivot.Rotation);
        Vector2 vecToObj = pointPos - pivot.Position;
		float dist = dir.Dot(vecToObj);

		return pivot.Position + (dist * dir);
	}
}
