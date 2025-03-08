using Godot;
using System;

public partial class MapButton : TextureButton
{
	[Export]
	PackedScene mapToLoad;

	[Export]
	int spawnPoint = -1;

	[Export]
	bool enabled = true;

	AnimationPlayer animPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		PivotOffset = new Vector2(Size.X / 2, Size.Y / 2);
        Scale = new Vector2(0.2f, 0.2f);

		if (mapToLoad == null) enabled = false;
		if (!enabled)
		{
			animPlayer.Stop();
			SelfModulate = new Color(0.3f,0.3f,0.3f);
        }
		GrabFocus();
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
		Globals.Instance.currentSpawnID = spawnPoint;
        GetTree().ChangeSceneToPacked(mapToLoad);
    }
}
