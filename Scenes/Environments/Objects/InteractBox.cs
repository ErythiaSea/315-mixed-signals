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
    [Export]
    public bool disablePlayerCam = false;

    public virtual void Interact(Player plrRef)
    {
        // Not interactable if inactive
        if (!active) return;

        if (ladderArea) {
            plrRef.autoWalk = true;
            plrRef.autoWalkDestinationX = Position.X;
            plrRef.toggleLadder();
        }

        if (scene == null) return;

        plrRef.setMovementState(MovementStates.MOVE_LOCKED);
        if (loadInCurrent)
        {
            Node2D instancedGame = (Node2D)scene.Instantiate();

            GetParent().AddChild(instancedGame);
            instancedGame.ZIndex = 10;
            plrRef.setMovementState(MovementStates.MOVE_LOCKED);
            if (disablePlayerCam)
            {
                Camera2D playerCam = plrRef.GetNode<Camera2D>("Camera2D");
                if (playerCam != null)
                {
                    GD.Print("we disablin");
                    playerCam.Enabled = false;
                }
            }
        }
            else
        {
            GetTree().ChangeSceneToPacked(scene);
        }
    }
}
