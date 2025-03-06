using Godot;
using System;

public partial class MapButton : TextureButton
{
	[Export]
	PackedScene mapToLoad;

	[Export]
	int spawnPoint = -1;

	[Export]
	bool spawnFacingLeft = false;

	float baseY;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (mapToLoad == null)
		{
			Disabled = true;
		}
		baseY = Position.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Disabled) return;

        //float t = (float)Time.GetTicksMsec() / 1000.0f;
        //Position.Y = baseY + Mathf.Sin(t) * 20.0f;
    }

    public void _OnPressed()
	{
		Globals.Instance.spawnFacingLeft = spawnFacingLeft;
		Globals.Instance.currentSpawnPoint = spawnPoint;
        GetTree().ChangeSceneToPacked(mapToLoad);
    }
}
