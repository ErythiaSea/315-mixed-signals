using Godot;
using System;

public partial class TutorialButton : TextureButton
{
	[Export]
	public String startID = "0";
	[Export]
	Panel dialogueBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (dialogueBox == null) {
			GD.PushWarning("No dialogue box was assigned to the tutorial button!");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Pressed()
	{
		dialogueBox.Call("start", startID);
	}
}
