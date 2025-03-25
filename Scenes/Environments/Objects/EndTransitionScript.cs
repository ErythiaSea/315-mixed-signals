using Godot;
using System;

public partial class EndTransitionScript : ColorRect
{
	 ShaderMaterial sMaterial;

	[Export]
	public float circleBlur = 0.01f;

	private bool hasClosed = false;
	private bool isClosing = false;
	public bool isEnding = false;
	private Player plr;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		plr = GetTree().Root.GetNode("Player") as Player;
		sMaterial = this.Material as ShaderMaterial;
		sMaterial.SetShaderParameter("circle_b", circleBlur);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        GlobalPosition = plr.GlobalPosition;
        if (isEnding)
		{
            if (!hasClosed)
            {
                if (!isClosing)
                {
                    transition(2f, 0f, 4f, true);
                    isClosing = true;
                }
            }
            else
            {
                //run the zz's and any dialogue
                GD.Print("ZZZ");
                //once completed run the transition func for opening again
            }
        }
	}

	private void transition(float from, float to, float duration, bool closed)
	{
		Tween transition = GetTree().CreateTween();
		transition.TweenMethod(Callable.From<float>(UpdateRadius), from, to, duration);
		transition.TweenProperty(this, "hasClosed", closed, 0f);
	}
	private void UpdateRadius(float rad)
	{
		sMaterial.SetShaderParameter("circle_r", rad);
	}
}
