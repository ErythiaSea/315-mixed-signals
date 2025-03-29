using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public enum GAMESTAGE
{
	BEGIN,
	TRANSPONDING,
	WAVEFORM,
	CONSTELLATION,
	TRANSLATION,
	END,
	TRANSITION
}

public enum GAMESTATE
{
	MENU,
	CUTSCENE,
	OVERWORLD,
	TRANSPOND,
	WAVEFORM,
	CONSTELLATION,
	TRANSLATION
}

public partial class Globals : Node
{
	// The instance of the Globals node that does GodotObject things a static
	// class otherwise cannot do (emit signals primarily)
	public static Globals Instance;

	// Progression stage property, backing field and update event
	[Signal]
	public delegate void ProgressionChangeEventHandler();
	private static GAMESTAGE _progressionStage;
	public static GAMESTAGE ProgressionStage
	{
		get
		{
			return _progressionStage;
		}
		set
		{
			_progressionStage = value;
			Instance.EmitSignal(SignalName.ProgressionChange);
		}
	}

	// Gamestate property, backing field and update event
	[Signal]
	public delegate void GamestateChangeEventHandler();
	private static GAMESTATE _gamestate;
	public static GAMESTATE Gamestate
	{
		get
		{
			return _gamestate;
		}
		set
		{	
			_gamestate = value;
			Instance.EmitSignal(SignalName.GamestateChange);
		}
	}

	// Day property, backing field and update signal
	[Signal]
	public delegate void DayChangedEventHandler();
	public static int Day { get; set; }

	// Tutorial progress property to track which tutorial should next be shown to the player
	public static GAMESTAGE TutorialProgress { get; set; } = GAMESTAGE.TRANSPONDING;

	// The spawn point you will appear at on level load
	public static int CurrentSpawnID { get; set; } = -1;

	// The colour used for the outline of interactable objects when a custom one is not set
	public static readonly Vector3 STANDARD_OUTLINE_COLOR = new (1.0f, 0.95f, 0.45f);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// enforce singleton, only one should exist at a time
		if (Instance != null)
		{
			QueueFree();
		}

		Instance = this;
		InitialGameSetUp();
	}

	public static void InitialGameSetUp()
	{
		Day = 0;
		ProgressionStage = GAMESTAGE.BEGIN;
	}

	public static void NewDay()
	{
		Day += 1;
		ProgressionStage = (Day == 2 ? GAMESTAGE.END : GAMESTAGE.TRANSPONDING);
		TranslationCanvasUI.CipherKey = 0;

		Radiotower.PivotRotationL = 0f;
		Radiotower.PivotRotationR = 0f;
		WaveformGame.LastAmplitude = 0f;
		WaveformGame.LastWavelength = 0f;

		Instance.EmitSignal(SignalName.DayChanged);
		GD.Print("Globals::NewDay complete");
	}
}
