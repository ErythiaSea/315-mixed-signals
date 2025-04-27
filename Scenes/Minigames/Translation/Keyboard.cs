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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TypeLetter(Button button)
	{
		answerBox.InsertTextAtCaret(button.Text);
	}
}
