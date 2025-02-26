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

	Globals globalScript;


    private string currentWord;
	Godot.Collections.Array<Node> containerNodes;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        globalScript = GetTree().Root.GetChild(1) as Globals;

        messageBox.Clear();
        answerBox.Clear();
        engHint.Clear();
        celHint.Clear();

		CallDeferred(nameof(TextInitalisation));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print(globalScript.wordList[1]); 
	}

	public void AnswerButton()
	{
		if(answerBox.Text == currentWord)
		{
			GD.Print("Winner Winner");
		}
		else
		{
			GD.Print("Doesnt feel right");
		}
	}
	private void TextInitalisation()
	{
        if (globalScript.wordArrayIndex != -1)
        {
			GD.Print("Assigned");
			GD.Print("word: " + globalScript.wordList[1]);
            currentWord = globalScript.wordList[0];
            messageBox.Text = ("[center] " + currentWord);

            HintsUpdate();
        }
        else
        {
			GD.Print("Not assigned");
            currentWord = "";
            messageBox.Text = currentWord;
        }
    }
	private void HintsUpdate()
	{
		string word = currentWord.ToLower();

		char[] charArray = word.ToCharArray();
		for(int i = 0; i < charArray.Length; i++)
		{
			celHint.AppendText(charArray[i].ToString() + "      =");
			celHint.Newline();
		}

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

            engHint.AppendText(((char)asciiValue).ToString());
            engHint.Newline();
        }
    }

	
}
