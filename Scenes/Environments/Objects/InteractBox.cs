using Godot;
using System;
using System.ComponentModel.Design;

public partial class InteractBox : Area2D
{
    [Export]
    PackedScene scene;

    [Export(PropertyHint.File)]
    public string scenePath;

    [Export]
    bool loadInCurrent = true;
    [Export]
    public bool active = true;
    [Export]
    public bool ladderArea = false;
    [Export]
    public bool disablePlayerCam = false;
    [Export]
    public GAMESTAGE requiredStage;

    bool isLeft;

    Globals globalScript;

    public override void _Ready()
    {
        GD.Print("Ready Func");
        globalScript = GetTree().Root.GetChild(1) as Globals;

        if (requiredStage == GAMESTAGE.TRANSITION) isLeft = GetNearestEdge();
    }
    public virtual void Interact(Player plrRef)
    {
        // Not interactable if inactive
        if (!active) return;
        if (!IsCorrectStage()) return;

        if (ladderArea) {
            plrRef.autoWalk = true;
            plrRef.autoWalkDestinationX = Position.X;
            plrRef.toggleLadder();
        }

        if (scene == null && scenePath == null) return;

        plrRef.setMovementState(MovementStates.MOVE_LOCKED);
        if (loadInCurrent)
        {
            CanvasItem instancedGame = (CanvasItem)scene.Instantiate();

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
            if (scene != null)
            {
                GetTree().ChangeSceneToPacked(scene);
            }
            else
            {
                GetTree().ChangeSceneToFile(scenePath);
            }
        }
    }

    private bool IsCorrectStage()
    {
        if(requiredStage == GAMESTAGE.TRANSITION) return true;

        if (requiredStage != globalScript.gameState.stage) return false;
        else return true;
        
    }

    private bool GetNearestEdge()
    {
       if(GlobalPosition.X < GetViewportRect().Size.X / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
