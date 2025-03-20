using Godot;
using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;

public partial class StarsParent : Node2D
{

    [Export]
    int randMin = -4;

    [Export]
    int randMax = 4;

    [Export]
    public float displaySpeed = 0.4f;

    // Called when the node enters the scene tree for the first time.

    private Godot.Collections.Array<Label> starLabels;
	public override void _Ready()
	{
        starLabels = GetLabels();
        GD.Print(starLabels.Count);
        DisplayNumbers();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

              
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

        } while (randNumArray.Sum() == 0);

        GD.Print("Valid random numbers");

        //updates global variable for cipher key in translation mechanic
        Globals.Instance.cipherKey = randNumArray.Sum();
        return randNumArray;

    }
    private Godot.Collections.Array<Label> GetLabels()
    {
        Godot.Collections.Array<Label> lbls = new Godot.Collections.Array<Label>();
        foreach (Node2D child in GetChildren())
        {
            Label current = child.GetChild(1) as Label;
            current.Modulate = new Color(current.Modulate.R, current.Modulate.G, current.Modulate.B, 0);
            lbls.Add(current);
        }

        return lbls;
    }
}
