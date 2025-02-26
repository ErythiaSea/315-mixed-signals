using Godot;
using System;

public partial class MapButton : TextureButton
{
	[Export]
	PackedScene mapToLoad;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (mapToLoad == null)
		{
			Disabled = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnPressed()
	{
        GetTree().ChangeSceneToPacked(mapToLoad);
    }
}
