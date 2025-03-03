using Godot;
using System;

public partial class DragParent : Control
{
    [Signal]
    public delegate void ConstellationCompletionEventHandler();

	Globals globalScript;
	bool isSignaled = false;
    Godot.Collections.Array<Node> sceneBars;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		globalScript = GetTree().Root.GetChild(1) as Globals;

		sceneBars = this.GetChildren(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (IsConstellationDrawn())
		{
            GD.Print("Constellation drawn");
			if (!isSignaled)
			{
                EmitSignal(SignalName.ConstellationCompletion);
				globalScript.gameState.stage = GAMESTAGE.TRANSLATION;
				isSignaled = true;
            }
        }
	}

	private bool IsConstellationDrawn()
	{
		DragProgressBar currentBarIndex;
		int completedCount = 0;
		for(int i = 0; i < sceneBars.Count; i++)
		{
			currentBarIndex = sceneBars[i] as DragProgressBar;

			if (currentBarIndex.isCompleted)
			{
				completedCount++;
			}
		}

		if(completedCount == sceneBars.Count)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
