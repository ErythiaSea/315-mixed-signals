using Godot;
using Godot.Collections;
using System;
using System.Linq;
using static Godot.Tween;

public partial class MapScreen : BaseMinigame
{
	Array<MapButton> buttonNodes = new Array<MapButton>{};
	bool goingToTravel = false;

	PackedScene travelLoading;
	private const string loadingPath = "res://Scenes/Loading/travel_loading.tscn";
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();

		Globals.PushGamestate(GAMESTATE.MAP);

        bool focusAssigned = false;
        foreach (Node child in GetNode("Buttons").GetChildren())
		{
			MapButton button = child as MapButton;
			buttonNodes.Add(button);
            button.Pressed += (() => OnButtonPress(button));

            // assign focus to the first enabled button
            if (!button.Disabled && focusAssigned) { 
				button.CallDeferred("grab_focus");
				focusAssigned = true;
			}
		}
		// assign focus to any button if none have it;
		if (!focusAssigned) { buttonNodes[0].CallDeferred("grab_focus"); }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (GetViewport().GuiGetFocusOwner() == null)
		{
			buttonNodes[Globals.Day].GrabFocus();
		}
	}

	void OnButtonPress(MapButton Cbutton)
	{
        if (Globals.Gamestate != GAMESTATE.MAP)
        {
            return;
        }
        
		if (Cbutton.requiredDay == Globals.Day)
		{
            foreach (MapButton button in buttonNodes)
            {
                button.Disabled = true;
            }
            player.EmitSignal("Transition", 2, 1.0f);
            exitTransition = TRANSITION.LEFTtoRIGHT;
            ResourceLoader.LoadThreadedRequest(loadingPath);
			goingToTravel = true;
            Close(); // this function is now named poorly due to this unforseen use case - erf
        }
		
    }

    protected override void OnTransitionFinish()
    {
		if (goingToTravel)
		{

            GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.LoadThreadedGet(loadingPath));
            Globals.SetGamestate(GAMESTATE.CUTSCENE);
		}
		else
		{
			Globals.PopGamestate(GAMESTATE.MAP);
		}
	}
}
