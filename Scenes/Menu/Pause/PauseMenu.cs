using Godot;
using System;
using System.Collections.Generic;

public partial class PauseMenu : Control
{
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
		resumeButton.Pressed += UnpauseGame;

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
	}

	private void UnpauseGame()
	{
        Hide();
        GetTree().Paused = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if ((@event.IsActionPressed("pause") || @event.IsActionPressed("ui_cancel")) && GetTree().Paused)
		{
			UnpauseGame();
			GetViewport().SetInputAsHandled();
		}
    }

    private void _On_OptionsButton_Pressed()
	{
		// spawn options menu
		Control optionsMenu = optionsScene.Instantiate<Control>();
		AddChild(optionsMenu);

		// when the options menu closes, return focus to options button
		optionsMenu.TreeExiting += () => optionsButton.GrabFocus();
	}
}
