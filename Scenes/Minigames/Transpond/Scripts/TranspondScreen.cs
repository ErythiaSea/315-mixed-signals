using Godot;
using System;

public partial class TranspondScreen : BaseMinigame
{
	// The strings used for starting the dialogue for tutorials for each minigame.
	[Export] String transpondTutorialStartID = "0";
	[Export] String waveformTutorialStartID = "1";
	[Export] String finalTutorialStartID = "2";

	Radiotower radiotower;
	WaveformGame waveform;
	Sprite2D leftBox, rightBox;
	Label radioLabel, waveLabel;
	Panel dialogueBox;
	TutorialButton tutorialButton;

	bool radiotowerComplete = false; bool waveformComplete = false;
	double exitTimer = 0;
	bool transitionedBetweenMinigames = false; float fadeTime = 0.0f;
	bool dialogueCalled = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		radiotower = GetNode<Radiotower>("radiotowerRoot");
		waveform = GetNode<WaveformGame>("waveformRoot");
		leftBox = GetNode<Sprite2D>("LeftBox");
		rightBox = GetNode<Sprite2D>("RightBox");
		radioLabel = GetNode<Label>("ControlsRadiotower");
		waveLabel = GetNode<Label>("ControlsWaveform");
		tutorialButton = GetNode<TutorialButton>("TutorialButton");

		// temp hopefully
		dialogueBox = GetNode<Panel>("DialogueBox");

		CheckStage();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		// Happens once, when 
		if (Globals.ProgressionStage == GAMESTAGE.WAVEFORM && !transitionedBetweenMinigames)
		{
			transitionedBetweenMinigames = true;
			waveLabel.Visible = true; radioLabel.Visible = false;

			// Fade the left minigame out of view, and the right one into view
			Tween tween = CreateTween();
			tween.TweenProperty(leftBox, "modulate", new Color(Colors.Black, 0.85f), 1);
			tween.TweenProperty(rightBox, "modulate", new Color(Colors.Black, 0.0f), 1);

			// Call waveform tutorial dialogue if this is the first time the player has reached it
			if (Globals.TutorialProgress <= GAMESTAGE.WAVEFORM)
			{
				dialogueBox.Call("start", waveformTutorialStartID);
				Globals.TutorialProgress = GAMESTAGE.CONSTELLATION;
				tutorialButton.startID = waveformTutorialStartID;
			}
		}

		if (Globals.ProgressionStage > GAMESTAGE.WAVEFORM)
		{
			//Have some indication of winning!
			exitTimer += delta;
			if (exitTimer > 2.5) Close();
			return;
		}

		// hacky code and will be removed in favour of Globals::Gamestate
		radiotower.gameActive = !dialogueBox.Visible;
		waveform.gameActive = !dialogueBox.Visible;
	}  

	private void CheckStage()
	{
		switch (Globals.ProgressionStage)
		{
			case GAMESTAGE.TRANSPONDING:
				Globals.PushGamestate(GAMESTATE.TRANSPOND);

				// Show tutorial if this is the first time entering this minigame
				if (Globals.TutorialProgress <= GAMESTAGE.TRANSPONDING)
				{
					dialogueBox.Call("start", transpondTutorialStartID);
					Globals.TutorialProgress = GAMESTAGE.WAVEFORM;
					tutorialButton.startID = transpondTutorialStartID;
				}
				break;

			case GAMESTAGE.WAVEFORM:
				Globals.PushGamestate(GAMESTATE.WAVEFORM);
				radiotower.CompletedPivots();
				waveLabel.Visible = true; radioLabel.Visible = false;
				leftBox.Visible = true; rightBox.Visible = false;
				tutorialButton.startID = waveformTutorialStartID;
				transitionedBetweenMinigames = true;
				break;

			default:
				radiotower.CompletedPivots();
				break;
		}
	}

	protected override void QuitMinigame()
	{
		Globals.PopGamestate();
		base.QuitMinigame();
	}
}
