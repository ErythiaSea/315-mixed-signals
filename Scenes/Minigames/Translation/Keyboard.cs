using Godot;
using System;

public partial class Keyboard : HBoxContainer
{
	[Export]
	Font buttonFont;
	[Export]
	LineEdit answerBox;
	[Export]
	Button confirmButton;
	[Export]
	Button deleteButton;

	public Button firstButton;
	Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = new Timer();
		AddChild(timer);
		timer.WaitTime = 0.5;
		timer.OneShot = true;
        timer.Timeout += OnTimerTimeout;

		firstButton = new();
		for (int i = 0; i < 26; i++)
		{
			Button kbButton = new Button();

			kbButton.Text = ((char)(i + 97)).ToString();
			kbButton.AddThemeColorOverride("font_color", Colors.Black);
            kbButton.AddThemeColorOverride("font_focus_color", Colors.DarkGoldenrod);
            kbButton.AddThemeFontOverride("font", buttonFont);
			kbButton.AddThemeFontSizeOverride("font_size", 90);
			kbButton.SizeFlagsHorizontal = SizeFlags.Expand;
			kbButton.Flat = true;
			kbButton.CustomMinimumSize = new Vector2(Mathf.Floor(Size.X / 26.0f), 0);
			kbButton.ClipText = true;

			AddChild(kbButton);
			kbButton.Pressed += () => TypeLetter(kbButton);
			GD.Print(kbButton.Size.X);

			if (i < 13)
			{
				kbButton.FocusNeighborBottom = confirmButton.GetPath();
				if (i == 0)
				{
					firstButton = kbButton;
					kbButton.GrabFocus();
				}
				if (i == 12)
				{
					confirmButton.FocusNeighborTop = kbButton.GetPath();
				}
			}
			else
			{
				kbButton.FocusNeighborBottom = deleteButton.GetPath();
				if (i == 13)
				{
					deleteButton.FocusNeighborTop = kbButton.GetPath();
				}
				if (i == 25)
				{
					kbButton.FocusNeighborRight = firstButton.GetPath();
					firstButton.FocusNeighborLeft = kbButton.GetPath();
				}
			}
		}
	}

    private void OnTimerTimeout()
    {
		timer.WaitTime = 0.5;
		Control focusOwner = GetViewport().GuiGetFocusOwner();
        if (focusOwner.GetParent() == this)
		{
			if (Input.IsActionPressed("ui_left"))
			{
				timer.WaitTime = 0.125;
				timer.Start();
				Control left = focusOwner.GetNode(focusOwner.FocusNeighborLeft) as Control;
				left.GrabFocus();
				return;
			}
			else if (Input.IsActionPressed("ui_left"))
			{
                timer.WaitTime = 0.125;
                timer.Start();
                Control right = focusOwner.GetNode(focusOwner.FocusNeighborRight) as Control;
				right.GrabFocus();
				return;
			}
		}
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_left") || @event.IsActionPressed("ui_right"))
		{
			if (GetViewport().GuiGetFocusOwner().GetParent() == this)
			{
				timer.Start();
			}
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void TypeLetter(Button button)
	{
		answerBox.InsertTextAtCaret(button.Text);
	}
}
