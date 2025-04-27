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
		Pressed += ShowDialogue;
	}

	// pressing top button summons the dialogue
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventJoypadButton button)
		{
			if (button.ButtonIndex == JoyButton.Y)
			{
				ShowDialogue();
			}
		}
	}

	// begin the tutorial dialogue unless the gamestate is dialogue or another indesirable state
	public void ShowDialogue()
	{
		if (Globals.Gamestate == GAMESTATE.DIALOGUE || Globals.Gamestate == GAMESTATE.CUTSCENE || Globals.Gamestate == GAMESTATE.MENU) { return; }
		dialogueBox.Call("start", startID);
	}
}
