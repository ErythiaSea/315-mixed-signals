using Godot;
using System;
using System.Linq;
using System.Text;

public partial class TranslationCanvasUI : BaseMinigame
{

	//Quick Inspector Refs, CHANGE THIS AT ANOTHER TIME, probably grab them specifcally from the borderDraw process
	[ExportGroup("Node References")]
	[Export]
	RichTextLabel messageBox;
	[Export]
	TextEdit answerBox;
	[Export]
	RichTextLabel celHint;
	[Export]
	RichTextLabel engHint;
	[Export]
	RichTextLabel cipherDisplay;
	[Export]
	Font englishFont;
	[Export]
	TextureRect winInd;

	[ExportGroup("Dialogue Paths")]
	[Export(PropertyHint.ResourceType, "DialogueData")]
	Resource Day1Dialogue;
	[Export(PropertyHint.ResourceType, "DialogueData")]
	Resource Day2Dialogue;

    Panel dialogueBox;
    Globals globalScript;
	CabinLevel cabin;

	private string currentWord;
	Godot.Collections.Array<Node> containerNodes;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		globalScript = Globals.Instance;
		cabin = GetParent<CabinLevel>();
		MinigameClosed += cabin.TranslationComplete;

		cipherDisplay.Clear();
		messageBox.Clear();
		answerBox.Clear();
		engHint.Clear();
		celHint.Clear();
		winInd.Visible = false;

		CallDeferred(nameof(TextInitalisation));

		if (Day1Dialogue == null) {
			GD.PrintErr("day 1 dialogue not assigned, please assign in inspector!");
		}
        if (Day2Dialogue == null)
        {
            GD.PrintErr("day 2 dialogue not assigned, please assign in inspector!");
        }

        // load dialogue data based on day
        dialogueBox = GetNode<Panel>("DialogueBox");
		dialogueBox.Connect("dialogue_ended", Callable.From(OnDialogueEnd));
		LoadDialogue();
		if (globalScript.tutorialProgress <= GAMESTAGE.TRANSLATION)
		{
			globalScript.tutorialProgress = GAMESTAGE.END;
			dialogueBox.Call("start", "tut");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public void AnswerButton()
	{
		string answer = answerBox.Text.StripEdges();
	
		if (globalScript.wordList[globalScript.gameState.day].NocasecmpTo(answer) == 0 || Input.IsActionPressed("middle_mouse"))
		{
			//POLISH IDEA: fade celestial text away and fade in the new word
			// you're telling me the poles came up with this idea? - erf
			messageBox.AddThemeFontOverride("normal_font", englishFont);
			messageBox.Text = ("[center] " + globalScript.wordList[globalScript.gameState.day]);
			globalScript.gameState.stage = GAMESTAGE.END;
			globalScript.isCurrentWordDone = true;

			//winInd.Visible = true;
			GetNode<VBoxContainer>("TextVBox").Visible = false;
			GetNode<VBoxContainer>("HintVBox").Visible = false;
			GetNode<Button>("CheckAnswerButton").Visible = false;
			cipherDisplay.Visible = false;
			canClose = false;
			dialogueBox.Call("start", "0");
		}
		else
		{
			dialogueBox.Call("start", "err");
			GD.Print("Doesnt feel right");
		}
	}
	private void TextInitalisation()
	{
		if (!globalScript.isCurrentWordDone)
		{
			GD.Print("CIPHER");
			string cWord = CipherWord(globalScript.wordList[globalScript.gameState.day]);
			messageBox.Text = ("[center] " + cWord);

			HintsUpdate(cWord);
			cipherDisplay.AppendText("[center] "+ (-globalScript.cipherKey).ToString());
		}
		else
		{
			messageBox.Text = "";
		}
	}
	private void HintsUpdate(string word)
	{
		char[] charArray = word.ToCharArray();
		for(int i = 0; i < charArray.Length; i++)
		{
			celHint.AppendText(charArray[i].ToString() + "      =");
			celHint.Newline();

			engHint.AppendText(charArray[i].ToString());
			engHint.Newline();
		}
	}
	private string CipherWord(string word)
	{
		string cipheredWord = null;
		string wordL = word.ToLower();

		char[] charArray = wordL.ToCharArray();
		
		for(int i = 0; i < charArray.Length; i++)
		{
			int asciiValue = charArray[i];
			GD.Print("Ascii value: " + asciiValue + " char: " + charArray[i].ToString());
			asciiValue -= 97;
			asciiValue = (asciiValue + globalScript.cipherKey) % 26;
			asciiValue += 97;

			

			cipheredWord += (char)asciiValue;
        }

		return cipheredWord;
	}

	private void OnDialogueEnd()
	{
		if (globalScript.isCurrentWordDone)
		{
			canClose = true;
			Close();
		}
	}

	private void LoadDialogue()
	{
		switch (globalScript.gameState.day)
		{
			case 0:
				dialogueBox.Set("data", Day1Dialogue);
				break;
			case 1:
				dialogueBox.Set("data", Day2Dialogue);
				break;
		}
	}
}
