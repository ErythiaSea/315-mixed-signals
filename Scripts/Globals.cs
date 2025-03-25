using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public enum GAMESTAGE
{
	TRANSPONDING,
	WAVEFORM,
	CONSTELLATION,
	TRANSLATION,
	END,
	TRANSITION
}

public struct GameState
{
	public GAMESTAGE stage;
	public int day;
}

public partial class Globals : Node
{
	public static Globals Instance;

	//Translation related Variables:

	//stores the cipher
	public int cipherKey = 0;
	//stores if the word has been done this loop or not
	public bool isCurrentWordDone = false;
	//Stores a list of words to be used in the translation, increments by the day the player is on
	public string[] wordList = { "Hot", "Cute" };


	//Transponding related variables:

	//Stores the last Completed pivots
	public float LpivotRotRef = 0f;
	public float RpivotRotRef = 0f;
	//Stores the last Completed wave
	public float waveAmpRef = 0f;
	public float waveLenRef = 0f;
	//State of the game, day and also stage the player is at
	public GameState gameState;

	//Spawning related variables:
	public int currentSpawnID = -1;
	public PackedScene nextMap;

	// To track which tutorials have automatically been displayed to the player
	public GAMESTAGE tutorialProgress = GAMESTAGE.TRANSPONDING;

	// The colour used for the outline of interactable objects when a custom one is not set
	public static Vector3 STANDARD_OUTLINE_COLOR { get; } = new Vector3(1.0f, 0.95f, 0.45f);

	public WipeTransition globalTransition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Instance = this;
		InitalGameSetUp();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void InitalGameSetUp()
	{
		gameState.day = 0;
		gameState.stage = GAMESTAGE.TRANSPONDING;

	}

	public void NewDay()
	{
		isCurrentWordDone = false;
		gameState.day += 1;
		gameState.stage = GAMESTAGE.TRANSPONDING;

		LpivotRotRef = 0f;
		RpivotRotRef = 0f;
		waveAmpRef = 0f;
		waveLenRef = 0f;

	}

	public void StartTransition(TRANSITION transitionType, float transitionLength)
	{
		if (globalTransition == null)
		{
			globalTransition = new WipeTransition();
			AddChild(globalTransition);
		}
	}
}
