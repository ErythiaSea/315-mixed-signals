using Godot;
using System;
using System.Linq;
using System.Text;

public partial class TranslationCanvasUI : CanvasLayer
{

	//Quick Inspector Refs, CHANGE THIS AT ANOTHER TIME, probably grab them specifcally from the borderDraw process
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

	Globals globalScript;


    private string currentWord;
	Godot.Collections.Array<Node> containerNodes;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        globalScript = GetTree().Root.GetChild(1) as Globals;

		cipherDisplay.Clear();
        messageBox.Clear();
        answerBox.Clear();
        engHint.Clear();
        celHint.Clear();
		winInd.Visible = false;

		CallDeferred(nameof(TextInitalisation));

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	
	}

	public void AnswerButton()
	{
		string answer = answerBox.Text.StripEdges();
	
		if (globalScript.wordList[globalScript.wordIndex].NocasecmpTo(answer) == 0)
		{
			//POLISH IDEA: fade celestial text away and fade in the new word
			messageBox.AddThemeFontOverride("normal_font", englishFont);
			messageBox.Text = ("[center] " + globalScript.wordList[globalScript.wordIndex]);
			globalScript.completeIndex++;
            winInd.Visible = true;
        }
		else
		{
			GD.Print("Doesnt feel right");
		}
	}
	private void TextInitalisation()
	{
		if (globalScript.wordIndex != -1 && globalScript.wordIndex != globalScript.completeIndex)
        {
			GD.Print("CIPHER");
			string cWord = CipherWord(globalScript.wordList[globalScript.wordIndex]);
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

			if ((asciiValue + globalScript.cipherKey) > 122)
            {
				int wrap = asciiValue - (asciiValue + globalScript.cipherKey);
                asciiValue = 97 - wrap;
            }
            else if ((asciiValue + globalScript.cipherKey) < 97)
            {
                int wrap = asciiValue - (asciiValue + globalScript.cipherKey);
                asciiValue = 122 - wrap;
            }
			else
			{
				asciiValue += globalScript.cipherKey;
			}

			cipheredWord += (char)asciiValue;
           
        }

		return cipheredWord;
    }

	
}
