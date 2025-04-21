using Godot;
using System;
using System.Collections.Generic;

public partial class PauseMenu : Control
{
	// scene to load on start button pressed
	// exported so designers can change it as, e.g., intro cutscenes are added
	[Export(PropertyHint.File, "*.tscn")]
	string startScene = null;

	// scene to instanciate on options button press
	[Export(PropertyHint.File, "*.tscn")]
	PackedScene optionsScene;

	Button resumeButton, optionsButton, quitButton;
	List<Button> buttons = new List<Button>{};
	double quitTimer = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		resumeButton = GetNode<Button>("ButtonContainer/ResumeButton");
		resumeButton.GrabFocus();
		resumeButton.Pressed += _On_ResumeButton_Pressed;

		optionsButton = GetNode<Button>("ButtonContainer/OptionsButton");
		optionsButton.Pressed += _On_OptionsButton_Pressed;

		quitButton = GetNode<Button>("ButtonContainer/QuitButton");

		buttons.Add(resumeButton);
		buttons.Add(quitButton);
		buttons.Add(optionsButton);

		if (optionsScene == null)
		{
			optionsScene = (PackedScene)ResourceLoader.Load("res://Scenes/Menu/Options/Options.tscn");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// quit button stuff
		if (quitButton.ButtonPressed)
		{
			quitTimer += delta;
			if (quitTimer > 0.5)
			{
				GetTree().Quit();
			}
		}
		else { quitTimer = 0; }

		// each button grabs and holds focus if mouse is over it (disables keyboard/controller control)
		foreach (Button button in buttons)
		{
			if (button.IsHovered())
			{
				button.GrabFocus();
			}
		}
	}

	private void UnpauseGame()
	{
        Hide();
        GetTree().Paused = false;
    }

	private void _On_ResumeButton_Pressed()
	{
		UnpauseGame();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause") && GetTree().Paused)
		{
			UnpauseGame();
			GetViewport().SetInputAsHandled();
		}
    }

    private void _On_OptionsButton_Pressed()
	{
		// spawn options menu
		Control optionsMenu = optionsScene.Instantiate<Control>();

		//optionsMenu.AnchorBottom = 0.5f;
		//optionsMenu.AnchorLeft = 0.5f;
		//optionsMenu.AnchorTop = 0.5f;
		//optionsMenu.AnchorRight = 0.5f;

		AddChild(optionsMenu);
	}
}
