using Godot;
using System;

public partial class MenuButton : Button
{
    [Export]
    Control relatedOption;
    [Export]
    Control relatedOptionFocus;

    private static readonly Color deselectColour = new Color(1, 1, 1, 0.66f);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetViewport().GuiFocusChanged += OnFocusChanged;

        // disable left/right input, lest you venture away to the
        // main menu and press buttons there
        FocusNeighborLeft = ".";
        FocusNeighborRight = ".";
	}

    private void OnFocusChanged(Control node)
    {
        if (node == this || node == relatedOption)
        {
            SelfModulate = Colors.White;
        }
        else
        {
            SelfModulate = deselectColour;
        }

        if (relatedOption != null && !relatedOption.HasFocus())
        {
            relatedOption.Hide();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public override void _Pressed()
    {
        if (relatedOption != null)
        {
            relatedOption.Show();
            if (relatedOptionFocus != null)
            {
                relatedOptionFocus.GrabFocus();
            }
            else
            {
                relatedOption.GrabFocus();
            }
        }
    }
}
