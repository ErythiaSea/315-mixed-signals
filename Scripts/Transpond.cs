using Godot;
using System;
using System.Linq;

public partial class Transpond : Node2D
{
	Sprite2D intersectIndicator;
	TranspondPivot lPivot, rPivot;

    Godot.Collections.Array<Node> towers;
    public Node currentTower;
	int idx = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		intersectIndicator = GetNode<Sprite2D>("intersectIndicator");
		lPivot = GetNode<TranspondPivot>("leftPivot");
        rPivot = GetNode<TranspondPivot>("rightPivot");

        towers = GetNode("towers").GetChildren();
        idx = (int)(GD.Randi() % 7);
		currentTower = towers[idx];
		GD.Print("len towers is: ", towers.Count);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 intersect = Vector2.Zero;
		if (lPivot.Rotation == rPivot.Rotation) intersectIndicator.Visible = false; // lines parallel
        else
		{
			//intersectIndicator.Visible = true;
			intersect = CalcIntersect();
			intersectIndicator.Position = intersect;
		}

		if (Input.IsActionJustPressed("print_intersect"))
		{
			GD.Print("Lines intersect at coords:" + intersect);
			if (lPivot.overlapsTower && rPivot.overlapsTower)
			{
				GD.Print("Tower found!");
				int ogIdx = idx;
				do { idx = (int)(GD.Randi() % 7); } while (idx == ogIdx);
				GD.Print(idx);
				currentTower = towers[idx];

				// show complete text
				Label wintext = GetNode<Label>("WinText");
				wintext.Visible = true;
			}
			else GD.Print("Nuh uh!");
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
}
