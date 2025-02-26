using Godot;
using System;

public partial class RestrictionBox : Area2D
{
	[Export]
	bool RestrictHorizontal = false;
	[Export]
	bool RestrictVertical = false;

    bool startChecked = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        // this in ready to see if player spawns in restriction box
        if (!startChecked)
        {
            foreach (Node2D body in GetOverlappingBodies())
            {
				Player_OLD plr = body as Player_OLD;
                if (plr != null)
                {
                    GD.Print("gaming");
                    plr.restrictHorizontal = RestrictHorizontal;
                    plr.restrictVertical = RestrictVertical;
                }
            }
            startChecked = true;
        }
	}

	void _OnBodyEntered(Node2D body)
	{
		Player_OLD plr = body as Player_OLD;
        if (plr != null)
        {
            plr.restrictHorizontal = RestrictHorizontal;
            plr.restrictVertical = RestrictVertical;
        }
    }

    void _OnBodyExited(Node2D body)
    {
		Player_OLD plr = body as Player_OLD;
        if (plr != null)
        {
            plr.restrictHorizontal = false;
            plr.restrictVertical = false;
        }
    }
}
