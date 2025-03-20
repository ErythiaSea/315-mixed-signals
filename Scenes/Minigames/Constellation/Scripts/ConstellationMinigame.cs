using Godot;
using System;

public partial class ConstellationMinigame : BaseMinigame
{
    double exitTimer = 0;
    Globals globalScript;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        base._Ready();

        globalScript = Globals.Instance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        base._Process(delta);

        if (globalScript.gameState.stage > GAMESTAGE.CONSTELLATION)
        {
            //Have some indication of winning!
            exitTimer += delta;
            if (exitTimer > 2.5) Close();
        }
    
    }
}
