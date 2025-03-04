using Godot;
using System;
using System.Linq;
using System.Reflection.Emit;

public partial class NumberDisplay : Control
{
	[Export]
	int randMin = -4;

	[Export]
	int randMax = 4;

	[Export]
	float displaySpeed = 0.4f;
	int index = 0;
	private float alphaChange;
	private bool hasNumbers = false;
	private bool isDisplayed = false;
	Godot.Collections.Array<Node> SceneLabels;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SceneLabels = this.GetChildren(false);

		SetNumbersVisibility();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (hasNumbers && !isDisplayed && index < SceneLabels.Count)
		{
			Godot.Label currentLabelIndex = SceneLabels[index] as Godot.Label;

			if (currentLabelIndex != null)
			{
                if (alphaChange <= 1)
                {
                    alphaChange += (float)delta * displaySpeed;
                    currentLabelIndex.Modulate = new Color(currentLabelIndex.Modulate.R, currentLabelIndex.Modulate.G, currentLabelIndex.Modulate.B, alphaChange);
                }
                else
                {
                    alphaChange = 0f;
                    index++;
                }
            }
			else
			{
				alphaChange = 0f;
				index = 0;
				isDisplayed = true;
			}
		}
	}

	public void DisplayNumbers()
	{
		Godot.Label currentLabelIndex = null;
		Godot.Collections.Array<int> randNumArray = RandomNumbers();

		for(int i = 0; i < SceneLabels.Count; i++)
		{
			currentLabelIndex = SceneLabels[i] as Godot.Label;

            currentLabelIndex.Text = string.Empty;
            currentLabelIndex.Text = randNumArray[i].ToString();
        }
		hasNumbers = true;
	}

	private Godot.Collections.Array<int> RandomNumbers()
	{
		Godot.Collections.Array<int> randNumArray = new Godot.Collections.Array<int>();
		do
		{
			randNumArray.Clear();

			for (int i = 0; i < SceneLabels.Count; i++)
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

	private void SetNumbersVisibility()
	{
		Godot.Label currentLabelIndex = null;
		for(int i = 0; i < SceneLabels.Count;i++)
		{
			currentLabelIndex = SceneLabels[i] as Godot.Label;
			currentLabelIndex.Modulate = new Color(currentLabelIndex.Modulate.R, currentLabelIndex.Modulate.G, currentLabelIndex.Modulate.B, 0);
		}
	}
}
