using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static Godot.WebSocketPeer;

public enum GAMESTAGE
{
	BEGIN = 0,
	TRANSPONDING = 1,
	WAVEFORM = 2,
	CONSTELLATION = 3,
	TRANSLATION = 4,
	END = 5,
	TRANSITION = 6
}

public enum GAMESTATE
{
	MENU = 0,
	CUTSCENE = 1,
	OVERWORLD = 2,
	TRANSPOND = 3,
	WAVEFORM = 4,
	CONSTELLATION = 5,
	TRANSLATION = 6,
	DIALOGUE = 7,
	NONE = 8,
	MAP = 9,
	PHOTOBOARD = 10
}

// i might need these for control state text later - eryth
[Flags]
public enum CONTEXTFLAGS
{
	CAN_INTERACT
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

	// Gamestate property, backing field, update event and print method
	[Signal]
	public delegate void GamestateChangeEventHandler();
	private static Stack<GAMESTATE> _gamestate = new();

	public static GAMESTATE Gamestate
	{
		get
		{
			return _gamestate.Peek();
		}
		private set
		{
			_gamestate.Clear();
			_gamestate.Push(value);
			Instance.EmitSignal(SignalName.GamestateChange);
		}
	}

	public static string GamestateString()
	{
		string gamestateString = "";
		foreach (var state in _gamestate)
		{
			gamestateString += state.ToString();
			gamestateString += ", ";
		}
		return gamestateString;
	}

	public static void PushGamestate(GAMESTATE state)
	{
		_gamestate.Push(state);
		GD.Print(state, " was pushed onto the gamestate stack. Stack is now: ", GamestateString());
		Instance.EmitSignal(SignalName.GamestateChange);
	}

	public static void PopGamestate(GAMESTATE state = GAMESTATE.NONE)
	{
		if (state == GAMESTATE.NONE)
		{
			GD.Print(_gamestate.Pop(), " was popped from the stack.");
			Instance.EmitSignal(SignalName.GamestateChange);
			return;
		}

		if (_gamestate.Peek() == state)
		{
			GD.Print(_gamestate.Pop(), " was popped from the gamestate stack. Stack is now: ", GamestateString());
			if (_gamestate.Count == 0) { GD.PushError("Gamestate stack was cleared! This should never happen."); }
			Instance.EmitSignal(SignalName.GamestateChange);
		}
		else
		{
			GD.PushWarning("Tried to remove ", state, " from the top of the stack, but it wasn't there. No change was made, stack is still: ", GamestateString());
		}
	}

	public static void SetGamestate(GAMESTATE state)
	{

		Gamestate = state;

		GD.Print("Gamestate was set; ", state, " is now the only gamestate.");
	}

	// Day property, backing field and update signal
	[Signal]
	public delegate void DayChangedEventHandler();
	public static int Day { get; set; }

	public int corvusLove = 0;
	public int cassioLove= 0;
	public int pyxisLove = 0;

    public ColorRect loadingScreen;
    
	public bool isGameDone = false;
	// Tutorial progress property to track which tutorial should next be shown to the player
	public static GAMESTAGE TutorialProgress { get; set; } = GAMESTAGE.TRANSPONDING;

	// The spawn point you will appear at on level load
	public static int CurrentSpawnID { get; set; } = -1;

	// The colour used for the outline of interactable objects when a custom one is not set
	public static readonly Vector3 STANDARD_OUTLINE_COLOR = new (1.0f, 0.95f, 0.45f);

	[Export]
	private Godot.Collections.Array<string> stateControlText = new();
	private RichTextLabel controlsText;

	private PauseMenu pauseMenu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		loadingScreen = GetChild(0).GetNode<ColorRect>("Loading");

		if (Engine.IsEditorHint())
		{
			//GetNode<CanvasLayer>("GlobalsCanvasLayer").Visible = false;
			return;
		}

		// enforce singleton, only one should exist at a time
		if (Instance != null)
		{
			QueueFree();
		}
		Instance = this;

		controlsText = GetNode<RichTextLabel>("GlobalsCanvasLayer/GlobalControl/ControlsText");
		Instance.GamestateChange += UpdateControlsText;

		InitialGameSetUp();

		pauseMenu = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/Pause/pause_menu.tscn").Instantiate<PauseMenu>();
		pauseMenu.Hide();
		GetNode<Control>("GlobalsCanvasLayer/GlobalControl").AddChild(pauseMenu);
	}

	public static void PauseGame()
	{
		Instance.pauseMenu.Show();
		Instance.GetTree().Paused = true;
		GD.Print("Game paused, current gamestate: ", Gamestate);
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

	public void updateAffection(String name, int value)
	{
		GD.Print(name, "  ", value);
		//comment for fucking github tweak
		switch (name)
		{
			case "CLOVE":
				corvusLove = value;
				break;
			case "CALOVE":
				cassioLove = value;
				break;
			case "PLOVE":
				pyxisLove = value;
				break;
		}
	}
	static public void UpdateControlsText()
	{
		GD.Print("updating controls text...");
		String newText = "";
		
		foreach (InputStruct input in InputManager.StateInputDict[Gamestate])
		{
			// ensure that the input matches our current control scheme
			if ((input.inputMethod == INPUT_METHODS.KEYBOARD_CONTROLLER)
				|| (input.inputMethod == INPUT_METHODS.KEYBOARD_ONLY && InputManager.IsController == false)
				|| (input.inputMethod == INPUT_METHODS.CONTROLLER_ONLY && InputManager.IsController))
			{
				newText += GetInputGlyphImage(input.glyphName) + input.inputText + "   ";
			}
		}
		Instance.controlsText.Text = newText;
	}

	static private string GetInputGlyphImage(string name)
	{
		const string keyFolder = "res://Sprites/InputKey/";
		string ctrlSuffix = "";
		switch (InputManager.ControllerType)
		{
			case GAMEPAD.KEYBOARD:
				ctrlSuffix = "_kb";
				break;
			case GAMEPAD.PS:
				ctrlSuffix = "_ps";
				break;
			default:
				ctrlSuffix = "_ps";
				break;
		}

		return "[img]" + keyFolder + name + ctrlSuffix + ".png[/img]";
	}

    public void FadeIn()
    {
		Tween fadeIn = CreateTween();
		fadeIn.TweenProperty(loadingScreen, "modulate", new Color(loadingScreen.Modulate.R, loadingScreen.Modulate.G, loadingScreen.Modulate.B, 1f), 0.8f);
    }

	public void FadeOut()
	{
        Tween fadeOut = CreateTween();
        fadeOut.TweenProperty(loadingScreen, "modulate", new Color(loadingScreen.Modulate.R, loadingScreen.Modulate.G, loadingScreen.Modulate.B, 0f), 0.8f);
    }

}
