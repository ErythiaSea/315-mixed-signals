using Godot;
using System;

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

		if (hasNumbers && !isDisplayed)
		{
			Label currentLabelIndex = SceneLabels[index] as Label;

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
		Label currentLabelIndex = null;
		for(int i = 0; i < SceneLabels.Count; i++)
		{
			currentLabelIndex = SceneLabels[i] as Label;
			RandomNumber(currentLabelIndex);
		}

		hasNumbers = true;
	}

	private void RandomNumber(Label label)
	{
        var rng = new RandomNumberGenerator();

        int randomNum = rng.RandiRange(randMin, randMax);
        label.Text = string.Empty;
        label.Text = randomNum.ToString();
    }

	private void SetNumbersVisibility()
	{
		Label currentLabelIndex = null;
		for(int i = 0; i < SceneLabels.Count;i++)
		{
			currentLabelIndex = SceneLabels[i] as Label;
			currentLabelIndex.Modulate = new Color(currentLabelIndex.Modulate.R, currentLabelIndex.Modulate.G, currentLabelIndex.Modulate.B, 0);
		}
	}
}
