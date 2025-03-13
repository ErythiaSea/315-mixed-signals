using Godot;
using System;

public partial class SignalBus : Node
{
	[Signal]
	public delegate void MinigameClosedEventHandler();

	[Signal]
	public delegate void DialogueClosedEventHandler();

	public static SignalBus Instance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
