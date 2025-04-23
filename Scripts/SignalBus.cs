using Godot;
using System;

public partial class SignalBus : Node
{
	[Signal]
	public delegate void DialogueStartedEventHandler();

	[Signal]
	public delegate void DialogueEndedEventHandler();

	public static SignalBus Instance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Instance != null) 
		{
			return;
		}

		Instance = this;
	}
}
