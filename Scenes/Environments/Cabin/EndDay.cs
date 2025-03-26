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
	Globals globalScript;
	bool isClosed = false;

	private bool isDisplayed = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().Root.GetChild(3).GetNode("Player") as Player;
		globalScript = Globals.Instance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (currentTrans != null) isClosed = currentTrans.isClosed;
		else return;

		if (currentTrans.isDone)
		{
           // dialogueBox.Call("start", "Sleep");
        }
	}

	public void EndTheDay()
	{
		globalScript.NewDay();
		player.SetMovementLock(true);

		if (!isClosed)
		{
            CreateTransition();
        }
	}

	public void startTheDay()
	{
		//open the circle back up
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
