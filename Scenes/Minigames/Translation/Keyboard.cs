using Godot;
using System;

public partial class Keyboard : HBoxContainer
{
	[Export]
	Font buttonFont;
	[Export]
	LineEdit answerBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Button first = new();
		for (int i = 0; i < 26; i++)
		{
			Button kbButton = new Button();

			kbButton.Text = ((char)(i + 97)).ToString();
			kbButton.AddThemeColorOverride("font_color", Colors.Black);
            kbButton.AddThemeColorOverride("font_focus_color", Colors.DarkGoldenrod);
            kbButton.AddThemeFontOverride("font", buttonFont);
			kbButton.AddThemeFontSizeOverride("font_size", 170);
			kbButton.SizeFlagsHorizontal = SizeFlags.Expand;
			kbButton.Flat = true;
			kbButton.CustomMinimumSize = new Vector2(Mathf.Floor(Size.X / 26.0f), 0);
			kbButton.ClipText = true;

			AddChild(kbButton);
			kbButton.Pressed += () => TypeLetter(kbButton);
			GD.Print(kbButton.Size.X);

			if (i == 0) { first = kbButton; }
			if (i == 25) { 
				kbButton.FocusNeighborRight = first.GetPath();
				first.FocusNeighborLeft = kbButton.GetPath();
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
