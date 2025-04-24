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

	private Godot.Collections.Array<Label> starLabels;
	private Godot.Collections.Array<StarNode> stars;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		stars = GetStars();
		starLabels = GetLabels();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Checks if the constellation has been completed, shows the whole constellation 
		//zoomed out if true
		if (IsConstellationComplete() && !hasSignalled)
		{
			GD.Print("Emitting signal...");
			EmitSignal(SignalName.ConstellationCompletion, GetConstellationCenter());
			hasSignalled = true;
		}	  
	}

	//Generates random numbers within a certain range and assigns them to 
	//a textbox which displays on star found
	public void GenerateNumbers()
	{
		Godot.Collections.Array<int> randNumArray = RandomNumbers();

		for (int i = 0; i < starLabels.Count; i++)
		{
			Label lbl = starLabels[i] as Label;
			lbl.Text = string.Empty;
			lbl.Text = randNumArray[i].ToString();
		}
	}

	//returns an array of numbers which all add up to a number thats not 0 
	//and between -4 and 4
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

		//updates global variable for cipher key in translation mechanic
		TranslationCanvasUI.CipherKey = randNumArray.Sum();
		return randNumArray;

	}
	
	//checks if all the stars completed then returns true if they are
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
	
	//gathers all labels attached to each star and stores them 
	//in an array
	private Godot.Collections.Array<Label> GetLabels()
	{
		Godot.Collections.Array<Label> lbls = new Godot.Collections.Array<Label>();
		foreach (Node2D child in GetChildren())
		{
			if (child is not StarNode) continue;
			Label current = child.GetChild(2) as Label;
			current.Modulate = new Color(current.Modulate.R, current.Modulate.G, current.Modulate.B, 0);
			lbls.Add(current);
		}

		return lbls;
	}

	// gathers all stars that are children of this parent,
	//stores them in array
	private Godot.Collections.Array<StarNode> GetStars()
	{
		Godot.Collections.Array<StarNode> strs = new Godot.Collections.Array<StarNode>();
		foreach (Node2D child in GetChildren())
		{
			GD.Print(child.Name);
			if (child as StarNode != null)
			{
				strs.Add(child as StarNode);
				GD.Print(child.Name, " is starnode");
			}
			else
			{
				GD.Print(child.Name, " is not starnode");
			}
		}

		GD.Print("stars count:", strs.Count());
		return strs;
	}

	// returns the center of the constellation for
	// the zoom out upon completeion of the constellation
	private Vector2 GetConstellationCenter()
	{
		float minX, maxX, minY, maxY;
		minX = maxX = stars[0].Position.X;
		minY = maxY = stars[0].Position.Y;

		for (int i = 1; i < stars.Count; i++)
		{
			Vector2 starPos = stars[i].Position;
			minX = Mathf.Min(minX, starPos.X);
			minY = Mathf.Min(minY, starPos.Y);
			maxX = Mathf.Max(maxX, starPos.X);
			maxY = Mathf.Max(maxY, starPos.Y);
		}
		GD.Print(Name, " i can do this");

		return new Vector2(minX + 0.5f*(maxX - minX), minY + 0.5f*(maxY - minY));
	}
}
