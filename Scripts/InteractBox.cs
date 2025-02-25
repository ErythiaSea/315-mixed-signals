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
    [Export]
    public bool ladderArea = false;

    public virtual void Interact(GravPlayer plrRef)
    {
        // Not interactable if inactive
        if (!active) return;

        if (ladderArea) {
            plrRef.autoWalk = true;
            plrRef.autoWalkDestinationX = Position.X;
            plrRef.toggleLadder();
        }

        if (scene == null) return;
        if (loadInCurrent)
        {
            Node2D instancedGame = (Node2D)scene.Instantiate();
            GetParent().AddChild(instancedGame);
            instancedGame.ZIndex = 3;
            plrRef.setMovementState(MovementStates.MOVE_LOCKED);
        }
        else
        {
            GetTree().ChangeSceneToPacked(scene);
        }
    }
}
