using Godot;
using System;
using System.Reflection;

public partial class EndTransitionScript : ColorRect
{

	[Export]
	public float circleBlur = 0.0f;

	[Export]
	public string textToDisplay = "Z..Z...Z";
	[Export]
	public double timeToDisplay = 0.4;

	public bool isClosed = false;
    public bool isDone = false;

    private ShaderMaterial sMaterial;
	private RichTextLabel sleepText;

	private double elaspsedTime = 0;
	private int textInd = 0;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		sleepText = GetChild(0) as RichTextLabel;
		sMaterial = this.Material as ShaderMaterial;
		sMaterial.SetShaderParameter("circle_b", circleBlur);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (isClosed && !isDone)
		{
			if(textInd < textToDisplay.Length)
			{
                if (elaspsedTime >= timeToDisplay)
                {
                    DisplayText(textInd);
                    elaspsedTime = 0;
                    textInd++;
                }
                else
                {
                    elaspsedTime += delta;
                }
            }
			else
			{
				fadeOutText();
				isDone = true;
            }
		}
       
	}

	//from radius to end radius of circle all within the specified range,
	//used for closing circle i.e 1.5f to 0f in 5f
	public void CloseCircle(float from, float to, float duration)
	{
		Tween transition = GetTree().CreateTween();
		transition.TweenMethod(Callable.From<float>(UpdateRadius), from, to, duration);
		transition.TweenProperty(this, "isClosed", true,0f);
	}

    //from radius to end radius of circle all within the specified range,
    //used for closing circle i.e 1.5f to 0f in 5f
	//then deletes itself
    public void OpenCircle(float from, float to, float duration)
    {
        Tween transition = GetTree().CreateTween();
        transition.TweenMethod(Callable.From<float>(UpdateRadius), from, to, duration);
        transition.TweenCallback(Callable.From(QueueFree));
    }

	public void DisplayText(int ind)
	{
		sleepText.AddText(textToDisplay[ind].ToString());
	}

	private void fadeOutText()
	{
		Tween fade = GetTree().CreateTween();
        fade.TweenProperty(sleepText, "modulate", new Color(sleepText.Modulate.R, sleepText.Modulate.G, sleepText.Modulate.B, 0f), 1f);
    }
	private void addText(int textSize)
	{
		GD.Print(textSize);
		sleepText.AddText((textToDisplay[textSize]).ToString());
	}
    private void UpdateRadius(float rad)
	{
		sMaterial.SetShaderParameter("circle_r", rad);
	}
}
