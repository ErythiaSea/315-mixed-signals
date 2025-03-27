using Godot;
using System;

public partial class MapButton : TextureButton
{
	[Export]
	PackedScene mapToLoad;
	[Export]
	public int requiredDay;
	[Export]
	Control dialogueBox;
	[Export]
	int spawnPoint = -1;

	AnimationPlayer animPlayer;
	MapScreen mapScreen;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mapScreen = GetParent().GetParent<MapScreen>();
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		PivotOffset = new Vector2(Size.X / 2, Size.Y / 2);
		Scale = new Vector2(0.2f, 0.2f);

		if (mapToLoad == null) Disabled = true;
		if (Disabled)
		{
			animPlayer.Stop();
			SelfModulate = new Color(0.3f,0.3f,0.3f);
			Disabled = true;
        }
		Pressed += _OnPressed;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//if (Disabled) return;

		if (HasFocus()) SelfModulate = Colors.Green;
		else SelfModulate = Colors.White;

		SelfModulate *= (!Disabled ? Colors.White : new Color(0.3f, 0.3f, 0.3f));
        //float t = (float)Time.GetTicksMsec() / 1000.0f;
        //Position.Y = baseY + Mathf.Sin(t) * 20.0f;
    }

	public void _OnPressed()
	{
		if (requiredDay == Globals.Instance.gameState.day)
		{
			GD.Print("day = " + Globals.Instance.gameState.day);
            GD.Print(this.Name, " pressed");
            if (Input.IsActionPressed("middle_mouse")) spawnPoint = 1;
            Globals.Instance.nextMap = mapToLoad; // pleeeeeeease don't pass by value!
            Globals.Instance.currentSpawnID = spawnPoint;
            //GetTree().ChangeSceneToPacked(mapToLoad);
        }
		else
		{
			dialogueBox.Call("start", "WrongDay");
		}
    }
}
