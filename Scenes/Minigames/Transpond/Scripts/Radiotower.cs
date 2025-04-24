using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;
using System.Linq.Expressions;

public partial class Radiotower : Node2D
{
	// super todo: stop having a bunch of l and r vars
	Sprite2D intersectIndicator;
	RadiotowerPivot lPivot, rPivot;

	Sprite2D leftIndicator, rightIndicator;
	float leftTimer, leftInterval, rightTimer, rightInterval;
	AudioEffectDistortion lDistort, rDistort;

	Godot.Collections.Array<Node> towers;
	public Node2D currentTower;
	int idx = 0;

	float winTimer = 0.0f;
	[Export] float winLengthRequirement = 1.25f;

	public bool gameActive = false;

	// Stores the pivot rotation value when transpond minigame is complete
	public static float PivotRotationL { get; set; } = 0f;
	public static float PivotRotationR { get; set; } = 0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		intersectIndicator = GetNode<Sprite2D>("intersectIndicator");
		leftIndicator = GetNode<Sprite2D>("leftIndicator");
		rightIndicator = GetNode<Sprite2D>("rightIndicator");
		lPivot = GetNode<RadiotowerPivot>("leftPivot");
		rPivot = GetNode<RadiotowerPivot>("rightPivot");

		int transpondLBusIndex = AudioServer.GetBusIndex("TranspondTowerL");
        int transpondRBusIndex = AudioServer.GetBusIndex("TranspondTowerR");
        lDistort = (AudioEffectDistortion)AudioServer.GetBusEffect(transpondLBusIndex, 0);
		rDistort = (AudioEffectDistortion)AudioServer.GetBusEffect(transpondRBusIndex, 0);

		towers = GetNode("towers").GetChildren();
		idx = (int)(GD.Randi() % 8);
		currentTower = (Node2D)towers[idx];
		GD.Print("len towers is: ", towers.Count);

		leftInterval = 0.75f; rightInterval = 0.75f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Globals.ProgressionStage != GAMESTAGE.TRANSPONDING || !gameActive)
		{
			lPivot.handleInputs = false;
			rPivot.handleInputs = false;
			return;
		}
		lPivot.handleInputs = true;
		rPivot.handleInputs = true;

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
		float ldist = closestPointOnLine.DistanceTo(currentTower.Position);
		lDistort.Drive = ((Mathf.Min(200.0f, ldist)) / 200.0f) * 0.75f;
		leftInterval = ((Mathf.Min(200.0f, ldist)+20.0f) / 220.0f) * 0.75f;

		closestPointOnLine = pointOnPivot(rPivot, currentTower.Position);
		float rdist = closestPointOnLine.DistanceTo(currentTower.Position);
		rDistort.Drive = ((Mathf.Min(200.0f, rdist)) / 200.0f) * 0.75f;
		rightInterval = ((Mathf.Min(200.0f, rdist) + 20.0f) / 220.0f) * 0.75f;

		// handle visual indicator stuff
		leftTimer += (float)delta; rightTimer += (float)delta;
		if (leftTimer > leftInterval) {
			leftIndicator.Visible = !leftIndicator.Visible;
			leftTimer = 0.0f;
		}
		if (rightTimer > rightInterval) {
			rightIndicator.Visible = !rightIndicator.Visible;
			rightTimer = 0.0f;
		}

		if (lPivot.overlapsTower && rPivot.overlapsTower)
		{
			winTimer += (float)delta;
			if (winTimer > winLengthRequirement)
			{
				winTimer = 0.0f;
				GD.Print("Tower found!");
				int ogIdx = idx;
				do { idx = (int)(GD.Randi() % 8); } while (idx == ogIdx);
				currentTower = (Node2D)towers[idx];

				//Updates the bars location for when how you see them when you play waveform after leaving and going back in again
				PivotRotationR = rPivot.Rotation;
				PivotRotationL = lPivot.Rotation;

				//Updates the stage of the game the player is at
				Globals.ProgressionStage = GAMESTAGE.WAVEFORM;

				// show complete text
				Label wintext = GetNode<Label>("WinText");
				wintext.Visible = true;
				
				gameActive = false;
				lPivot.handleInputs = false;
				rPivot.handleInputs = false;
				lPivot.streamPlayer.Stop();
				rPivot.streamPlayer.Stop();
			}
		}
		else winTimer = 0.0f;

		if (Input.IsActionPressed("interact"))
		{
			GD.Print("ldist: ", ldist, " rdist: ", rdist);
			GD.Print("left overlap:", lPivot.overlapsTower, " right overlap: ", rPivot.overlapsTower);
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

	public void Close()
	{
		Player plr = GetNode<Player>("../Player");
		plr.SetMovementLock(false);
		QueueFree();
	}

	public void CompletedPivots()
	{
		rPivot.Rotation = PivotRotationR;
		lPivot.Rotation = PivotRotationL;
		lPivot.handleInputs = false;
		rPivot.handleInputs = false;
	}
}
