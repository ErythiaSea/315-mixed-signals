using Godot;
using System;

public partial class InteractBox : Area2D
{
    [Export]
    PackedScene scene;

    [Export]
    bool loadInCurrent = true;
    [Export]
    public bool active = true;

    public virtual void Interact(Player plrRef)
    {
        if (!active) return;
        if (loadInCurrent)
        {
            Node2D instancedGame = (Node2D)scene.Instantiate();
            GetParent().AddChild(instancedGame);
            instancedGame.ZIndex = 3;
        }
        else
        {
            GetTree().ChangeSceneToPacked(scene);
        }
    }
}
