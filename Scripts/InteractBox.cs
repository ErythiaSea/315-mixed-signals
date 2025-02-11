using Godot;
using System;

public partial class InteractBox : Area2D
{
    [Export]
    PackedScene minigame;

    public virtual void Interact(Player plrRef)
    {
        Node instancedGame = minigame.Instantiate();
        GetParent().AddChild(instancedGame);
    }
}
