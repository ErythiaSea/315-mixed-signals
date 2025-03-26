using Godot;
using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;

public partial class StarsParent : Node2D
{
	[Signal]
	public delegate void ConstellationCompletionEventHandler(Vector2 centerStar);

	[Export]
	int randMin = -4;

	[Export]
	int randMax = 4;

	[Export]
	public float displaySpeed = 0.4f;


	public bool hasSignalled = false;
	// Called when the node enters the scene tree for the first time.

	private Godot.Collections.Array<Label> starLabels;

	private Godot.Collections.Array<StarNode> stars;

	public override void _Ready()
	{
		stars = GetStars();
		starLabels = GetLabels();
		GD.Print(stars.Count);
		DisplayNumbers();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (IsConstellationComplete() && !hasSignalled)
		{
			EmitSignal(SignalName.ConstellationCompletion, GetCenterStar());
			hasSignalled = true;
		}
			  
	}
	public void DisplayNumbers()
	{
		Godot.Collections.Array<int> randNumArray = RandomNumbers();

		for (int i = 0; i < starLabels.Count; i++)
		{
			Label lbl = starLabels[i] as Label;
			lbl.Text = string.Empty;
			lbl.Text = randNumArray[i].ToString();
		}
	}

	private Godot.Collections.Array<int> RandomNumbers()
	{
		Godot.Collections.Array<int> randNumArray = new Godot.Collections.Array<int>();
		do
		{
			randNumArray.Clear();

			for (int i = 0; i < starLabels.Count; i++)
			{
				var rng = new RandomNumberGenerator();
				int randomNum = rng.RandiRange(randMin, randMax);

				randNumArray.Add(randomNum);
			}

		} while (randNumArray.Sum() == 0 && randNumArray.Sum() < -4 && randNumArray.Sum() > 4);

		GD.Print("Valid random numbers");

		//updates global variable for cipher key in translation mechanic
		Globals.Instance.cipherKey = randNumArray.Sum();
		return randNumArray;

	}
	
	private bool IsConstellationComplete()
	{
		int starsCompleted = 0;

		for(int i = 0; i < stars.Count; i++)
		{
			if (stars[i].isFound)
			{
				starsCompleted++;
			}
		}

		if (starsCompleted == stars.Count) return true;

		return false;
	}
	private Godot.Collections.Array<Label> GetLabels()
	{
		Godot.Collections.Array<Label> lbls = new Godot.Collections.Array<Label>();
		foreach (Node2D child in GetChildren())
		{
			Label current = child.GetChild(2) as Label;
			current.Modulate = new Color(current.Modulate.R, current.Modulate.G, current.Modulate.B, 0);
			lbls.Add(current);
		}

		return lbls;
	}

	private Godot.Collections.Array<StarNode> GetStars()
	{
		Godot.Collections.Array<StarNode> strs = new Godot.Collections.Array<StarNode>();
		foreach (Node2D child in GetChildren())
		{
			strs.Add(child as StarNode);
		}

		return strs;
	}
	private Vector2 GetCenterStar()
	{
		int bestCount = 0;
		StarNode bestStar = null;
		for (int i = 0; i < stars.Count; i++)
		{
			if (stars[i].adjacentStars.Count > bestCount)
			{
				bestCount = stars[i].adjacentStars.Count;
				bestStar = stars[i];
			}
		}

		return bestStar.GlobalPosition;
	}
}
