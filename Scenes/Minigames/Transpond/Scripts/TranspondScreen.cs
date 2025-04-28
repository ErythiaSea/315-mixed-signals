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
	Panel dialogueBox;
	TutorialButton tutorialButton;
	AnimationPlayer animPlayer;

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
		tutorialButton = GetNode<TutorialButton>("UICanvas/TutorialButton");
		animPlayer = GetNode<AnimationPlayer>("minigameCompleteAnims");

		dialogueBox = GetNode<Panel>("UICanvas/DialogueBox");

        waveform.WaveformComplete += OnWaveformComplete;
		CheckStage();
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		base._Process(delta);

		// Happens once, when the transpond minigame is completed
		if (Globals.ProgressionStage == GAMESTAGE.WAVEFORM && !transitionedBetweenMinigames)
		{
            Tween cameraPan = CreateTween();
            cameraPan.Parallel().TweenProperty(this, "offset", new Vector2(-400, Offset.Y), 1f);
            //dialogueBox.Position = new Vector2(600f, dialogueBox.Position.Y);

            transitionedBetweenMinigames = true;

			// Fade the left minigame out of view, and the right one into view
			Tween tween = CreateTween();
			tween.TweenProperty(leftBox, "modulate", new Color(Colors.Black, 0.85f), 1);
			tween.TweenProperty(rightBox, "modulate", new Color(Colors.Black, 0.0f), 1);

			// Call waveform tutorial dialogue if this is the first time the player has reached it
			if (Globals.TutorialProgress <= GAMESTAGE.WAVEFORM)
			{
				Globals.PopGamestate(GAMESTATE.TRANSPOND);
				Globals.PushGamestate(GAMESTATE.WAVEFORM);

				dialogueBox.Call("start", waveformTutorialStartID);
				Globals.TutorialProgress = GAMESTAGE.CONSTELLATION;
				tutorialButton.startID = waveformTutorialStartID;
			}
		}

		//if (Globals.ProgressionStage > GAMESTAGE.WAVEFORM)
		//{
		//	//Have some indication of winning!
		//	exitTimer += delta;
		//	if (exitTimer > 3.5) Close();
		//	return;
		//}

		// hacky code and will be removed in favour of Globals::Gamestate
		radiotower.gameActive = !dialogueBox.Visible;
		waveform.gameActive = !dialogueBox.Visible;
	}  

	private void CheckStage()
	{
		Tween cameraPan = CreateTween();
		switch (Globals.ProgressionStage)
		{
			case GAMESTAGE.TRANSPONDING:
				Globals.PushGamestate(GAMESTATE.TRANSPOND);

				cameraPan.Parallel().TweenProperty(this, "offset", new Vector2(400, Offset.Y), 1f);
				//dialogueBox.Position = new Vector2(0f,dialogueBox.Position.Y);
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

                cameraPan.Parallel().TweenProperty(this, "offset", new Vector2(-400, Offset.Y), 1f);
                //dialogueBox.Position = new Vector2(500f, dialogueBox.Position.Y);

                radiotower.CompletedPivots();
				leftBox.Visible = true; rightBox.Visible = false;
				tutorialButton.startID = waveformTutorialStartID;
				transitionedBetweenMinigames = true;
				break;

			default:
				Globals.PushGamestate(GAMESTATE.TRANSPOND);
				radiotower.CompletedPivots();
				break;
		}
	}

    private void OnWaveformComplete()
    {
		// Plays the final animation, waits 2.5s then closes
		string animKey = (Globals.Day == 0 ? "Day1Complete" : "Day2Complete");
        animPlayer.Play(animKey);
		animPlayer.AnimationFinished += (StringName) => {
			GetTree().CreateTimer(2.5).Timeout += Close;
		};
    }

	protected override void QuitMinigame()
	{
		Globals.PopGamestate();
		base.QuitMinigame();
	}
}
