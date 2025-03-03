using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public enum GAMESTAGE
{
	INITAL,
	TRANSPONDING,
	WAVEFORM,
	CONSTELLATION,
	TRANSLATION,
	END,
	TRANSITION
}

public struct GameState
{
	public GAMESTAGE stage;
	public int day;
}
public partial class Globals : Node
{
    
    public int cipherKey = 0;
	public bool isCurrentWordDone = false;
	public bool hasGameStarted = false;

	public GameState gameState;

	public string[] wordList = { "Hot", "Cute" };

}
