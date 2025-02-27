using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Globals : Node
{
    public int cipherKey = 3;
	public int wordArrayIndex = 0;
	public string[] wordList = { "Hot", "Beansman" };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

}
