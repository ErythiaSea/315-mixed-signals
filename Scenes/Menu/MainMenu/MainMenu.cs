using Godot;
using System;
using System.Collections.Generic;

public partial class MainMenu : Control
{
	// scene to load on start button pressed
	// exported so designers can change it as, e.g., intro cutscenes are added
	[Export(PropertyHint.File, "*.tscn")]
	string startScene = null;

	// scene to instanciate on options button press
	[Export(PropertyHint.File, "*.tscn")]
	PackedScene optionsScene;

	Button startButton, optionsButton, quitButton;
	List<Button> buttons = new List<Button>{};
	double quitTimer = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// main menu should never be on top of something so we're safe to set the gamestate
		Globals.SetGamestate(GAMESTATE.MENU);

		startButton = GetNode<Button>("Margins/ButtonContainer/StartButton");
		startButton.GrabFocus();
		startButton.Pressed += _On_StartButton_Pressed;

		optionsButton = GetNode<Button>("Margins/ButtonContainer/OptionsButton");
		optionsButton.Pressed += _On_OptionsButton_Pressed;

		quitButton = GetNode<Button>("Margins/ButtonContainer/QuitButton");

		buttons.Add(startButton);
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

	private void _On_StartButton_Pressed()
	{
		// grab the cabin scene as fallback
		Globals.InitialGameSetUp();
		if (startScene == null)
		{
			startScene = "res://Scenes/Environments/Cabin/cabin.tscn";
		}

		GetTree().ChangeSceneToFile(startScene);
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
