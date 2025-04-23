using Godot;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

public partial class EndDay : Node2D
{
	[Export]
	PackedScene endTransition;
	[Export]
	float transitionTime = 3f;

	[ExportSubgroup("Dialogue")]
	// the dialogue box to trigger
	[Export]
	Control dialogueBox;

	private EndTransitionScript currentTrans;
	Player player;
	bool isClosed = false;

	private bool isDisplayed = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		isDisplayed = false;
		dialogueBox.Connect("dialogue_ended", Callable.From(startTheDay));
		player = GetTree().Root.GetChild(-1).GetNode("Player") as Player;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (currentTrans != null) isClosed = currentTrans.isClosed;
		else return;

		if (currentTrans.isDone && !isDisplayed)
		{
		   dialogueBox.Call("start", "SLEEP");
			isDisplayed = true;
		}
	}

	public void EndTheDay()
	{
		GD.Print("called");
		player.SetMovementLock(true);
		Globals.PushGamestate(GAMESTATE.CUTSCENE);

		if (!isClosed)
		{
			CreateTransition();
		}
	}

	public void startTheDay()
	{
		Globals.NewDay();
		currentTrans.OpenCircle(0f, 1f, transitionTime);
		player.SetMovementLock(false);
		isClosed = false;
		Globals.PopGamestate(GAMESTATE.CUTSCENE);
	}

	private void CreateTransition()
	{
		EndTransitionScript transition = endTransition.Instantiate() as EndTransitionScript;
		//transition.PivotOffset = new Vector2(transition.Size.X / 2,transition.Size.Y / 2);
		player.AddChild(transition);
		currentTrans = transition;
		transition.CloseCircle(1f, 0f, transitionTime);
	}
}
