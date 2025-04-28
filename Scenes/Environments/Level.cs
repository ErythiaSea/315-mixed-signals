using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class Level : Node2D
{
	// The list of spawn points for this level
	[Export]
	Array<SpawnData> spawnData;

	// Whether the player camera is enabled in this level or not.
	// If false, the player camera can never be enabled, but it can still be disabled if true (for minigames)
	[Export]
	bool cameraEnabled = true;

	[Export]
	float cameraZoom = 1.0f;

	[ExportGroup("Camera Offsets")]
	[Export]
	float OffsetX = 0f;
	[Export] 
	float OffsetY = 0f;

	[Export]
	float musicFadeDb = 20;

	// The region the camera can move around in.
	// If this is left untouched the camera is disabled
	[ExportGroup("Camera Limits")]
	[Export]
	int leftLimit = 0;
	[Export]
	int topLimit = 0;
	[Export]
	int rightLimit = 0;
	[Export]
	int bottomLimit = 0;
	
	Player player;
	PackedScene pauseScene;

	private AudioStreamPlayer sceneMusic;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (GetNode("SceneMusic") != null)
		{
			sceneMusic = GetNode("SceneMusic") as AudioStreamPlayer;
		}
		// don't execute this in editor
		if (Engine.IsEditorHint()) return;

		player = GetNode<Player>("Player");

		int spawnID = Globals.CurrentSpawnID;
		if (spawnID >= 0 && spawnData.Count > 0)
		{
			player.Position = spawnData[spawnID].spawnPosition;
			player.SetSpriteFlipH(!spawnData[spawnID].faceLeft);
			GD.Print("we spawn");
		}
		Globals.CurrentSpawnID = -1;

		// force the camera to be disabled if limits are untouched (to mitigate user error)
		if (leftLimit == rightLimit && leftLimit == topLimit && leftLimit == bottomLimit) {
			cameraEnabled = false;
		}

		if (cameraEnabled)
		{
			player.SetCameraLimits(leftLimit, topLimit, rightLimit, bottomLimit);
			player.setCameraOffsets(OffsetX, OffsetY);
			player.SetCameraZoom(cameraZoom);
		}
		else
		{
			player.SetCameraEnabled(false);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Creates an editor warning if no Player child is found, or more than one exists
	// This technically doesn't check if the node is actually of type Player, but that's an engine limitation
	public override string[] _GetConfigurationWarnings()
	{
		List<String> warnings = new List<String>();

		Array<Node> playerChildren = FindChildren("Player", "CharacterBody2D");
		if (playerChildren.Count == 0) { warnings.Add("This level needs a Player child node called Player!"); }
		if (playerChildren.Count > 1) { warnings.Add("This level has more than one Player child, which may break things!"); }

		string[] warnArray = warnings.ToArray();
		return warnArray;
	}

	public bool getCameraEnabled() { return cameraEnabled; }

	public override void _UnhandledInput(InputEvent input)
	{
		if (input.IsActionPressed("close") && !GetTree().Paused && Globals.Gamestate == GAMESTATE.OVERWORLD)
		{
			GD.Print("pausing game...");
			Globals.PauseGame();
			GetViewport().SetInputAsHandled();
		}
	}
	public void FadeOutMusic()
	{
		Tween fade = GetTree().CreateTween();
		fade.TweenProperty(sceneMusic, "volume_db", (sceneMusic.VolumeDb - musicFadeDb), 1f);
	}

	public void FadeInMusic()
	{
		GD.Print("fadeeed");
		Tween fade = GetTree().CreateTween();
		fade.TweenProperty(sceneMusic, "volume_db",(sceneMusic.VolumeDb + musicFadeDb) , 1f);
	}
}
