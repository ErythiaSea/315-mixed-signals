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

	private AnimatedSprite2D calenderAnim;
    private ShaderMaterial sMaterial;
	private RichTextLabel sleepText;

	private double elaspsedTime = 0;
	private int textInd = 0;
	private bool isAnimating = false;
	private bool hasTextDisplayed = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		calenderAnim = GetNode("Calender") as AnimatedSprite2D;
		sleepText = GetChild(0) as RichTextLabel;
		sMaterial = this.Material as ShaderMaterial;
		sMaterial.SetShaderParameter("circle_b", circleBlur);

		calenderAnim.AnimationFinished += EndAnim;
		calenderAnim.Animation = Globals.Day.ToString();
		calenderAnim.Frame = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (isClosed && !hasTextDisplayed)
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
				hasTextDisplayed = true;
            }
		}

		if (hasTextDisplayed && !isAnimating)
		{
            Tween calender = GetTree().CreateTween();
			calender.TweenInterval(1.0f);
            calender.TweenProperty(calenderAnim, "modulate", new Color(calenderAnim.Modulate.R, calenderAnim.Modulate.G, calenderAnim.Modulate.B, 1f), 2f);

            calender.TweenCallback(Callable.From(() => calenderAnim.Play()));
			isAnimating = true;
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

    public void EndAnim()
    {
		GD.Print("anim done");
        Tween animEnd = GetTree().CreateTween();
        animEnd.TweenProperty(calenderAnim, "modulate", new Color(calenderAnim.Modulate.R, calenderAnim.Modulate.G, calenderAnim.Modulate.B, 0f), 2f);
		isDone = true;
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
