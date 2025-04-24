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

	Control topPage, quitPage, creditPage;
	Control currentPage;

	Button startButton, optionsButton, creditsButton, quitMenuButton;
	Button quitGameButton, quitBackButton, creditBackButton;

	List<Button> buttons = new List<Button>{};
	Stack<Control> focusStack;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// main menu should never be on top of something so we're safe to set the gamestate
		Globals.SetGamestate(GAMESTATE.MENU);

		topPage = GetNode<Control>("Top");
		creditPage = GetNode<Control>("Credits");
		quitPage = GetNode<Control>("Quit");

		currentPage = topPage;

		startButton = topPage.GetNode<Button>("ButtonContainer/StartButton");
		startButton.GrabFocus();
		startButton.Pressed += _On_StartButton_Pressed;

		optionsButton = topPage.GetNode<Button>("ButtonContainer/OptionsButton");
		optionsButton.Pressed += _On_OptionsButton_Pressed;

		creditsButton = topPage.GetNode<Button>("ButtonContainer/CreditsButton");
		creditBackButton = creditPage.GetNode<Button>("BackButton");
		creditsButton.Pressed += () => ForwardPage(creditPage, creditBackButton);
		creditBackButton.Pressed += BackwardPage;

		quitMenuButton = topPage.GetNode<Button>("ButtonContainer/QuitButton");
		quitGameButton = quitPage.GetNode<Button>("QuitButton");

		quitMenuButton.Pressed += () => ForwardPage(quitPage, quitGameButton);
		quitGameButton.Pressed += () => GetTree().Quit();
		quitPage.GetNode<Button>("BackButton").Pressed += BackwardPage;

		buttons.Add(startButton);
		buttons.Add(quitMenuButton);
		buttons.Add(optionsButton);

		if (optionsScene == null)
		{
			optionsScene = (PackedScene)ResourceLoader.Load("res://Scenes/Menu/Options/Options.tscn");
		}

		// create the focus stack
		focusStack = new Stack<Control>();
	}

	private void ForwardPage(Control page, Control focusReceiver)
	{
		// Add the button that opens a new page to the focus stack
		focusStack.Push(GetViewport().GuiGetFocusOwner());

		// hide the previous page, show the new page and set as current
		currentPage.Visible = false;
		page.Visible = true;
		currentPage = page;

		// the first button of the new page will get focus
		focusReceiver.GrabFocus();
	}

	private void BackwardPage()
	{
		// hide the previous page, show the new page and set as current
		currentPage.Visible = false;
		topPage.Visible = true;
		currentPage = topPage;

		// return focus to whatever button put us on that page
		focusStack.Pop().GrabFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Go back if the back button is pressed and we're not on the top page
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (currentPage != topPage)
			{
				BackwardPage();
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
