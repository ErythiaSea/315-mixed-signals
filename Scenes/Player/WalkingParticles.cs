using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class WalkingParticles : GpuParticles2D
{
	private float timeElapsed = 0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if(timeElapsed >= 1f)
		{
			QueueFree();
		}
		else
		{
			timeElapsed += (float)delta;
		}

	}
}
