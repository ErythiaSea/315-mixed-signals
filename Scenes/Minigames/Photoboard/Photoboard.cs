using Godot;
using System;

public partial class Photoboard : BaseMinigame
{
    public override void _Ready()
    {
        base._Ready();
        CabinLevel cabin = GetParent<CabinLevel>();
        Globals.PushGamestate(GAMESTATE.PHOTOBOARD);
        MinigameClosed += cabin.PhotoboardClosed;
    }

	protected override void QuitMinigame()
	{
        Globals.PopGamestate(GAMESTATE.PHOTOBOARD);
		base.QuitMinigame();
	}
}
