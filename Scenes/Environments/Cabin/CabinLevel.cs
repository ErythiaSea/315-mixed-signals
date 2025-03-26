using Godot;
using System;

// todo: the fact that any of this exists makes me profusely sad
// it shouldnt but it does. it might be too late to get rid of anyway. the damage is done - eryth
[Tool]
public partial class CabinLevel : Level
{
	// the dialogue box where post-translation text shows up
	[Export]
	Control dialogueBox;
	[Export]
	String translationEndStartID = "translationend";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public void TranslationComplete()
	{
		GD.Print("translation complete");
		if (Globals.Instance.gameState.stage == GAMESTAGE.END)
		{
			dialogueBox.Call("start", translationEndStartID);
		}
	}
}
