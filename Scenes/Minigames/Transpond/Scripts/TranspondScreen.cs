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
	bool fade = false; float fadeTime = 0.0f;
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
		if (Globals.ProgressionStage == GAMESTAGE.WAVEFORM)
		{
			waveLabel.Visible = true; radioLabel.Visible = false;
			fade = true;
			if (Globals.TutorialProgress <= GAMESTAGE.WAVEFORM)
			{
				dialogueBox.Call("start", waveformTutorialStartID);
				Globals.TutorialProgress = GAMESTAGE.CONSTELLATION;
				tutorialButton.startID = waveformTutorialStartID;
			}
		}

		if (fade)
		{
			fadeTime += (float)delta;
			if (fadeTime >= 0.85f) { fade = false; fadeTime = 0.85f; }
			leftBox.Modulate = new Color(Colors.Black, fadeTime);
			rightBox.Modulate = new Color(Colors.Black, 0.85f - fadeTime);
		}

		if (Globals.ProgressionStage > GAMESTAGE.WAVEFORM)
		{
			//Have some indication of winning!
			exitTimer += delta;
			if (exitTimer > 2.5) Close();
			return;
		}

		radiotower.gameActive = !dialogueBox.Visible;
		waveform.gameActive = !dialogueBox.Visible;
	}  

	private void CheckStage()
	{
		switch (Globals.ProgressionStage)
		{
			case GAMESTAGE.TRANSPONDING:
				GD.Print("trans");
				if (Globals.TutorialProgress <= GAMESTAGE.TRANSPONDING)
				{
					dialogueBox.Call("start", transpondTutorialStartID);
					Globals.TutorialProgress = GAMESTAGE.WAVEFORM;
					tutorialButton.startID = transpondTutorialStartID;
				}
				break;
			case GAMESTAGE.WAVEFORM:
				GD.Print("wave");
				radiotower.CompletedPivots();
				waveLabel.Visible = true; radioLabel.Visible = false;
				fade = true;
				tutorialButton.startID = waveformTutorialStartID;
				break;
			default:
				GD.Print("Default");
				radiotower.CompletedPivots();
				break;
		}
	}
}
