using Godot;
using System;

public partial class Photoboard : BaseMinigame
{
    public override void _Ready()
    {
        base._Ready();
        CabinLevel cabin = GetParent<CabinLevel>();
        MinigameClosed += cabin.PhotoboardClosed;
    }
}
