using Godot;
using System;
using System.Linq;
using System.Text;

public partial class TranslationCanvasUI : BaseMinigame
{
	// The words that can appear in translation
	[Export]
	private string[] wordList = { "Hot", "Cute" };

	//Quick Inspector Refs, CHANGE THIS AT ANOTHER TIME, probably grab them specifcally from the borderDraw process
	[ExportGroup("Node References")]
	[Export]
	RichTextLabel messageBox;
	[Export]
	LineEdit answerBox;
	[Export]
	Label cipherDisplay;
	[Export]
	Font englishFont;
	[Export]
	BaseButton answerButton;
	[Export]
	BaseButton backButton;
	[Export]
	Keyboard keyboard;

	[ExportGroup("Dialogue Paths")]
	[Export(PropertyHint.ResourceType, "DialogueData")]
	Resource Day1Dialogue;
	[Export(PropertyHint.ResourceType, "DialogueData")]
	Resource Day2Dialogue;

	Panel dialogueBox;
	CabinLevel cabin;

	public static int CipherKey { get; set; }

	private string currentWord;
	Godot.Collections.Array<Node> containerNodes;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Globals.PushGamestate(GAMESTATE.TRANSLATION);
		answerBox.GrabFocus();
		cabin = GetParent<CabinLevel>();

		MinigameClosed += cabin.TranslationComplete;
		answerBox.TextSubmitted += (string text) => AnswerButton();
		answerButton.Pressed += AnswerButton;
		backButton.Pressed += answerBox.DeleteCharAtCaret;

		//cipherDisplay.Clear();
		cipherDisplay.Text = "";
		messageBox.Clear();
		answerBox.Clear();
		//engHint.Clear();
		//celHint.Clear();
		//winInd.Visible = false;

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

		// call tutorial dialogue if it's the first time doing translation
		// otherwise, put focus where it needs to be
		if (Globals.TutorialProgress <= GAMESTAGE.TRANSLATION)
		{
			Globals.TutorialProgress = GAMESTAGE.END;
			dialogueBox.Call("start", "tut");
		}
		else
		{
			CallDeferred(MethodName.PutFocusSomewhere);
		}
	}

	public void AnswerButton()
	{
		string answer = answerBox.Text.StripEdges();
	
		if (wordList[Globals.Day].NocasecmpTo(answer) == 0 || Input.IsActionPressed("middle_mouse"))
		{
			//POLISH IDEA: fade celestial text away and fade in the new word
			// you're telling me the poles came up with this idea? - erf
			messageBox.AddThemeFontOverride("normal_font", englishFont);
			messageBox.Text = ("[center] " + wordList[Globals.Day]);
			Globals.ProgressionStage = GAMESTAGE.END;

			//winInd.Visible = true;
			//GetNode<VBoxContainer>("TextVBox").Visible = false;
			//GetNode<VBoxContainer>("HintVBox").Visible = false;
			//GetNode<Button>("CheckAnswerButton").Visible = false;
			answerButton.Hide();
			backButton.Hide();
			cipherDisplay.Visible = false;
			canClose = false;

			Node2D victAnim = GetNode<Node2D>("victAnim");
			victAnim.Show();

			string animName = Globals.Day == 0 ? "Plumage" : "Compass";
			AnimatedSprite2D victSprite = victAnim.GetNode<AnimatedSprite2D>("victSprite");
			victSprite.Play(animName);
			victSprite.AnimationFinished += () => {
				GetTree().CreateTimer(1.5).Timeout += () =>
				{
					GetNode<Control>("godDialogue").Show();
					dialogueBox.Call("start", "0");
				};
			};
		}
		else
		{
			dialogueBox.Call("start", "err");
			GD.Print("Doesnt feel right");
		}
	}

	private void TextInitalisation()
	{
		if (Globals.ProgressionStage > GAMESTAGE.TRANSLATION) {
			messageBox.Text = "";
			return;
		}

		GD.Print("CIPHER");
		string cWord = CipherWord(wordList[Globals.Day]);
		messageBox.Text = ("[center]" + cWord);

		//HintsUpdate(cWord);
		//cipherDisplay.AppendText("[center] "+ (-CipherKey).ToString());
		cipherDisplay.Text = (-CipherKey).ToString();
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

			asciiValue = ((asciiValue - 97 + CipherKey) % 26) + 97;

			cipheredWord += (char)asciiValue;
		}

		return cipheredWord;
	}

	private void OnDialogueEnd()
	{
		if (Globals.ProgressionStage > GAMESTAGE.TRANSLATION)
		{
			canClose = true;
			Close();
			return;
		}

		// assign focus when tutorial dialogue box closes
		PutFocusSomewhere();
	}

	private void LoadDialogue()
	{
		switch (Globals.Day)
		{
			case 0:
				dialogueBox.Set("data", Day1Dialogue);
				break;
			case 1:
				dialogueBox.Set("data", Day2Dialogue);
				break;
		}
	}

    private void PutFocusSomewhere()
    {
        if (InputManager.IsController)
        {
            keyboard.firstButton.GrabFocus();
        }
        else
        {
            answerBox.GrabFocus();
        }
    }

    protected override void QuitMinigame()
    {
		Globals.Instance.CallDeferred(Globals.MethodName.PopGamestate, Variant.From((int)GAMESTATE.TRANSLATION));
        base.QuitMinigame();
    }
}
