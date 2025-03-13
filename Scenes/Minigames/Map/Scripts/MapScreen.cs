using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class MapScreen : BaseMinigame
{
	Array<MapButton> buttonNodes = new Array<MapButton>{};
	bool p = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node child in GetNode("Buttons").GetChildren())
		{
			buttonNodes.Add(child as MapButton);
		}
		buttonNodes[0].CallDeferred("grab_focus");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
