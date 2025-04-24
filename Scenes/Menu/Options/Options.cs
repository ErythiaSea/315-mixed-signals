using Godot;
using System;
using System.Collections.Generic;

public partial class Options : Control
{
    VBoxContainer currentPage;
    Stack<Control> focusStack;

	Button closeButton, audioButton, controlsButton;
	VBoxContainer topPage, audioPage, controlsPage;

	Button masterButton, musicButton, sfxButton, envButton;
	Button swapABButton;

	Label headerLabel;

	const string masterBusName = "Master";
	const string musicBusName = "Music";
	const string sfxBusName = "SFX";
	const string envBusName = "Environmental";
	int masterBusIdx, musicBusIdx, sfxBusIdx, envBusIdx;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// get header label
		headerLabel = GetNode<Label>("Margins/HeaderLabel");

		// get all pages
		topPage = GetNode<VBoxContainer>("Margins/OptionsTop");
        audioPage = GetNode<VBoxContainer>("Margins/OptionsAudio");
        controlsPage = GetNode<VBoxContainer>("Margins/OptionsControls");

		// show only top page on ready
		topPage.Visible = true; audioPage.Visible = false; controlsPage.Visible = false;
		currentPage = topPage;

		// get top page buttons
        audioButton = topPage.GetNode<Button>("AudioButton");
		controlsButton = topPage.GetNode<Button>("ControlsButton");
        closeButton = topPage.GetNode<Button>("BackButton");

		// connect top page buttons to respective functions
        audioButton.GrabFocus();
		audioButton.Pressed += () => ForwardPage(audioPage);
		controlsButton.Pressed += () => ForwardPage(controlsPage);
        closeButton.Pressed += CloseOptions;

		// get bus indexes
        masterBusIdx = AudioServer.GetBusIndex(masterBusName);
        musicBusIdx = AudioServer.GetBusIndex(musicBusName);
        sfxBusIdx = AudioServer.GetBusIndex(sfxBusName);
        envBusIdx = AudioServer.GetBusIndex(envBusName);

		// connect audio sliders to updatevolume func with respective bus index
        audioPage.GetNode<Slider>("MasterSlider").ValueChanged += (value) => UpdateVolume(masterBusIdx, value);
        audioPage.GetNode<Slider>("MusicSlider").ValueChanged += (value) => UpdateVolume(musicBusIdx, value);
        audioPage.GetNode<Slider>("SFXSlider").ValueChanged += (value) => UpdateVolume(sfxBusIdx, value);
        audioPage.GetNode<Slider>("EnvironmentalSlider").ValueChanged += (value) => UpdateVolume(envBusIdx, value);

        // last button in each page is back, connect to backwardpage
        audioPage.GetChild<Button>(-1).Pressed += BackwardPage;
        controlsPage.GetChild<Button>(-1).Pressed += BackwardPage;

        GetViewport().GuiFocusChanged += OnFocusChange;
		
		// create the focus stack
        focusStack = new Stack<Control>();

		// play fade in anim
        OpenOptions();
	}

    private void OnFocusChange(Control node)
    {
    }

    private void ForwardPage(VBoxContainer page)
	{
		// Add the button that opens a new page to the focus stack
		focusStack.Push(GetViewport().GuiGetFocusOwner());

		// hide the previous page, show the new page and set as current
		currentPage.Visible = false;
		page.Visible = true;
		currentPage = page;

		// the first button of the new page will get focus
		page.GetChild<MenuButton>(0).GrabFocus();
		UpdateHeaderLabel();
	}

	private void BackwardPage()
	{
        // hide the previous page, show the new page and set as current
        currentPage.Visible = false;
		topPage.Visible = true;
		currentPage = topPage;

		// return focus to whatever button put us on that page
		focusStack.Pop().GrabFocus();
		UpdateHeaderLabel();
	}

	private void UpdateHeaderLabel()
	{
		string labelText = "";
		if (currentPage == topPage)
		{
			labelText = "Options";
		}
		else if (currentPage == controlsPage)
		{
			labelText = "Control Options";
		}
		else if (currentPage == audioPage)
		{
			labelText = "Audio Options";
		}
		headerLabel.Text = labelText;
	}

	// fade in the options menu
	private void OpenOptions()
	{
		Modulate = new Color(1, 1, 1, 0);
		Tween tween = CreateTween();
		tween.TweenProperty(this, "modulate", Colors.White, 0.25f);
	}

	// fade out the options menu
	private void CloseOptions()
	{
        Tween tween = CreateTween();
		tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 0.25f);
		tween.TweenCallback(Callable.From(QueueFree));
	}

	private void UpdateVolume(int busIdx, double value)
	{
		AudioServer.SetBusVolumeDb(busIdx, (float)Mathf.LinearToDb(value));
		GD.Print("new volume of bus ", AudioServer.GetBusName(busIdx), " is: ", AudioServer.GetBusVolumeDb(busIdx));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Go back or close options if the back button is pressed
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (currentPage == topPage)
			{
				CloseOptions();
			}
			else
			{
				BackwardPage();
			}
		}
	}
}
