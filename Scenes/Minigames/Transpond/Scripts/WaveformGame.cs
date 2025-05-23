using Godot;
using System;

public partial class WaveformGame : Node2D
{
	// Stores the wave amplitude and wavelength when waveform minigame is complete
	public static float LastAmplitude { get; set; } = 0f;
	public static float LastWavelength { get; set; } = 0f;

	WaveRender playerWave, realWave, ghostWave;
	float targetWavelength, targetAmplitude;

	[Export]
	float ampTolerance = 2.0f;
	[Export]
	float wlTolerance = 2.0f;
	float alignedTimer = 0.0f;
	bool tunedSignal = false;

	public bool gameActive = false;
	AnimatedSprite2D victoryAnim;

	[Signal]
	public delegate void WaveformCompleteEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerWave = GetNode<WaveRender>("playerwave");
		realWave = GetNode<WaveRender>("realwave");
		ghostWave = GetNode<WaveRender>("ghostwave");
		newWavelength();

		victoryAnim = GetNode<AnimatedSprite2D>("victoryAnim");
		victoryAnim.Hide();
	}

	void newWavelength()
	{
		if(LastAmplitude == 0f)
		{
			targetWavelength = (GD.Randf() * 42.5f) + 7.5f;
			realWave.wavelength = targetWavelength;
			ghostWave.wavelength = targetWavelength;

			targetAmplitude = (GD.Randf() * 115.0f) + 10.0f;
			realWave.amplitude = targetAmplitude;
			ghostWave.amplitude = targetAmplitude;
			tunedSignal = false; alignedTimer = 0.0f;
		}
		else
		{
			realWave.wavelength = LastWavelength;
			realWave.amplitude = LastAmplitude;

			playerWave.wavelength = LastWavelength;
			playerWave.amplitude = LastAmplitude;
			ghostWave.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Globals.ProgressionStage != GAMESTAGE.WAVEFORM || !gameActive) return;

		float wlChange = 0.5f;
		float wlMult = (playerWave.wavelength / 100.0f);

		bool inputThisFrame = false;
		if (Input.IsActionPressed("left_pivot_ccw"))
		{
			playerWave.amplitude += 0.5f;
			inputThisFrame = true;
		}
		if (Input.IsActionPressed("left_pivot_cw"))
		{
			playerWave.amplitude -= 0.5f;
			inputThisFrame = true;
		}
		if (Input.IsActionPressed("right_pivot_ccw"))
		{
			playerWave.wavelength += wlChange * wlMult;
			inputThisFrame = true;
		}
		if (Input.IsActionPressed("right_pivot_cw"))
		{
			playerWave.wavelength -= wlChange * wlMult;
			inputThisFrame = true;
		}

		playerWave.amplitude = Mathf.Clamp(playerWave.amplitude, 0.0f, 125.0f);
		playerWave.wavelength = Mathf.Clamp(playerWave.wavelength, 5.0f, 50.0f);
		
		// debug
		//if (Input.IsActionJustPressed("interact"))
		//{
		//	GD.Print("target w: ", targetWavelength, ", current: ", playerWave.wavelength);
		//	GD.Print(Mathf.Abs(playerWave.wavelength - targetWavelength));
		//	GD.Print("target a:", targetAmplitude, ", current: ", playerWave.amplitude);
		//	GD.Print(Mathf.Abs(playerWave.wavelength - targetWavelength));
		//}

		if (tunedSignal)
		{
			GD.Print("Task complete!");

			LastAmplitude = realWave.amplitude;
			LastWavelength = realWave.wavelength;
			ghostWave.Visible = false;

			//Updates the stage that the player is on
			Globals.ProgressionStage = GAMESTAGE.CONSTELLATION;

			// show completion
			playerWave.waveColor = Colors.Green;
			gameActive = false;

			// tween out waves
			Tween tween = CreateTween();
			tween.Parallel().TweenProperty(playerWave, "self_modulate:a", 0, 0.5);
			tween.Parallel().TweenProperty(realWave, "self_modulate:a", 0, 0.5);

			// fade out waveform lines and decal / overlay modulation change
			victoryAnim.Show();
			victoryAnim.Play();
			victoryAnim.AnimationFinished += () => EmitSignal(SignalName.WaveformComplete);
		}

		// wavelength and amplitude are within range, no input this frame
		if (Mathf.Abs(playerWave.wavelength - targetWavelength) < wlTolerance
			&& Mathf.Abs(playerWave.amplitude - targetAmplitude) < ampTolerance
			&& inputThisFrame == false)
		{
			alignedTimer += (float)delta;
			if (alignedTimer > 1.0f) tunedSignal = true;
		}
		else alignedTimer = 0.0f;
	}
}
