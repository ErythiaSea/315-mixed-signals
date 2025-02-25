using Godot;
using System;

public partial class Globals : Node
{
    public int cipherKey = 0;
	Godot.Collections.Array<String> WordList;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		WordList.Add("Hot");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
