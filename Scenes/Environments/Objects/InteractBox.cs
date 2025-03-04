using Godot;
using System;

public partial class InteractBox : Area2D
{
    // whether the interact box can be used
    [Export]
    public bool active = true;

    [ExportGroup("Scene Loading")]
    // the scene to load (for minigames)
    [Export]
    PackedScene scene;

    // temp for scenes that reference other scenes (usually through an object of this class)
    // to prevent circular dependencies. todo: make this suck less
    [Export(PropertyHint.File, "*.tscn")]
    public string scenePath;

    // swap to this scene or instanciate it in the current
    [Export]
    bool loadInCurrent = true;

    // if the interact box should disable the player camera or not (so that minigames with
    // their own camera correctly use that camera). todo: minigames as control nodes so they
    // automatically center on screen
    [Export]
    public bool disablePlayerCam = false;

    [ExportGroup("World")]
    // if the interact box should toggle the player's ladder state or not
    [Export]
    public bool ladderArea = false;

    // the dialogue box to trigger
    [Export]
    Panel dialogueBox;

    // the start id for the dialogue
    [Export]
    String startID;

    public virtual void Interact(Player plrRef)
    {
        // Not interactable if inactive
        if (!active) return;

        if (ladderArea) {
            plrRef.isAutoWalking = true;
            plrRef.autoWalkDestinationX = Position.X;
            plrRef.toggleLadder();
        }

        if (dialogueBox != null) {
            dialogueBox.Call("start", startID);
            plrRef.setMovementLock(true);
            return;
        }

        if (scene == null && scenePath == null) return;

        if (loadInCurrent)
        {
            CanvasItem instancedGame = (CanvasItem)scene.Instantiate();

            GetParent().AddChild(instancedGame);
            instancedGame.ZIndex = 10;
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
}
