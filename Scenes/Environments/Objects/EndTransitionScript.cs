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
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sMaterial = this.Material as ShaderMaterial;
		sMaterial.SetShaderParameter("circle_b", circleBlur);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
       
       
	}

	public void CloseCircle(float from, float to, float duration)
	{
		Tween transition = GetTree().CreateTween();
		transition.TweenMethod(Callable.From<float>(UpdateRadius), from, to, duration);
	}
    public void OpenCircle(float from, float to, float duration)
    {
        Tween transition = GetTree().CreateTween();
        transition.TweenMethod(Callable.From<float>(UpdateRadius), from, to, duration);
        transition.TweenCallback(Callable.From(QueueFree));
    }
    private void UpdateRadius(float rad)
	{
		sMaterial.SetShaderParameter("circle_r", rad);
	}
}
