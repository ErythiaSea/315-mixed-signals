using Godot;
using System;

public partial class EndDay : Node2D
{
    [Export]
    PackedScene endTransition;
	[Export]
	float transitionTime = 3f;

    private EndTransitionScript currentTrans;
    Player player;
	Globals globalScript;
	bool isClosed = false;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().Root.GetChild(3).GetNode("Player") as Player;
		globalScript = Globals.Instance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(currentTrans != null)
		{
			GD.Print("UPDATING");
			currentTrans.GlobalPosition = player.GlobalPosition;
		}

		if (isClosed)
		{
			//run whatever we need here
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
        GetTree().CreateTimer(transitionTime);
		isClosed = true;
	}

	public void startTheDay()
	{
		//open the circle back up
	}

	private void CreateTransition()
	{
		EndTransitionScript transition = endTransition.Instantiate() as EndTransitionScript;
		GetParent().AddChild(transition);
		currentTrans = transition;
		transition.CloseCircle(2f, 0f, transitionTime);
    }
}
