using Godot;
using Godot.Collections;
using System;

// todo: all of this code is preeeetty bad and should NOT exist in final - eryth
public partial class GameEndTemp : Control
{
    [Export]
    BaseButton corvusButton;
    [Export]
    string corvusDialogueID = "0";

    [Export]
    BaseButton pyxisButton;
    [Export]
    string pyxisDialogueID = "1";

    [Export]
    BaseButton cassButton;
    [Export]
    string cassDialogueID = "2";

    Panel dialogueBox;
    Button mainMenuButton;
    Label tempLabel; // well theyre all temp arent they

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        dialogueBox = GetNode<Panel>("DialogueBox");
        dialogueBox.Connect("dialogue_ended", Callable.From(OnDialogueClose));

        mainMenuButton = GetNode<Button>("MainMenuButton");
        mainMenuButton.Pressed += (() => GetTree().ChangeSceneToFile("res://Scenes/Menu/MainMenu/main_menu.tscn"));

        tempLabel = GetNode<Label>("TempLabel");

        bool focusAssigned = false;
        foreach (Node child in GetNode("ButtonsContainer").GetChildren())
        {
            BaseButton button = child as BaseButton;
            GD.Print(button.Name);
            button.Pressed += (() => OnButtonPressed(button));

            // assign focus to the first enabled button
            if (!button.Disabled && focusAssigned)
            {
                button.CallDeferred(MethodName.GrabFocus);
                focusAssigned = true;
            }
        }
        // assign focus to any button if none have it;
        if (!focusAssigned) { corvusButton.CallDeferred(MethodName.GrabFocus); }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    void OnButtonPressed(BaseButton button)
    {
        // i did try a switch case
        string dialogueID = null;
        if (button == pyxisButton)
        {
            dialogueID = pyxisDialogueID;
        }
        if (button == corvusButton)
        {
            dialogueID = corvusDialogueID;
        }
        if (button == cassButton)
        {
            dialogueID = cassDialogueID;
        }

        dialogueBox.Call("start", dialogueID);
    }

    void OnDialogueClose()
    {
        GetNode<HBoxContainer>("ButtonsContainer").Visible = false;
        mainMenuButton.Visible = true;
        tempLabel.Text = "Game over!\nThanks for playing!";
    }
}
