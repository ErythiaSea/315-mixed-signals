using Godot;
using System;

public partial class InteractBox : Area2D
{
    [Export]
    PackedScene minigame;

    public virtual void Interact(Player plrRef)
    {
        Node2D instancedGame = (Node2D)minigame.Instantiate();
        GetParent().AddChild(instancedGame);
        instancedGame.ZIndex = 3;
    }
}
