using Godot;
using System;

public partial class InteractBox : Area2D
{
    // whether the interact box can be used
    [Export]
    public bool active = true;

    [ExportGroup("Scene Loading")]
    // the scene to load (for minigames)
    //[Export]
    //PackedScene scene;

    // temp for scenes that reference other scenes (usually through an object of this class)
    // to prevent circular dependencies. todo: make this suck less
    [Export(PropertyHint.File, "*.tscn")]
    public string scenePath = null;

    // the above scenePath once loaded will be stored here
    private PackedScene scene = null;

    // swap to this scene (level transition) or instanciate it in the current
    [Export]
    bool loadInCurrent = true;

    // the spawn point ID that the player will load in at (if this is a level transition)
    // if this is < 0 the player's default scene position will be used instead
    [Export]
    int spawnPoint = -1;

    [Export]
    bool spawnFacingLeft = false;

    // if the interact box should disable the player camera or not (so that minigames with
    // their own camera correctly use that camera). todo: minigames as control nodes so they
    // automatically center on screen
    [Export]
    public bool disablePlayerCam = false;

    [ExportGroup("World")]
    // if the interact box should toggle the player's ladder state or not
    [Export]
    public bool isLadderArea = false;

    // if the interact box should become inactive after use
    [Export]
    bool isOneShot = false;

    // if the interact box triggers immediately when entered
    [Export]
    public bool isAutofire = false;

    [ExportSubgroup("Dialogue")]
    // the dialogue box to trigger
    [Export]
    Panel dialogueBox;

    // the start id for the dialogue
    [Export]
    String startID;

    // if the dialogue box should lock the player's movement
    [Export]
    bool lockPlayerMovement = false;

    public override void _Ready()
    {
        // possible todo: for things that will have a transition before getting the loaded file, we can
        // move the load request to Interact() so that other levels aren't loaded in memory the whole time
        if (scenePath != null)
        {
            ResourceLoader.LoadThreadedRequest(scenePath);
        }
    }

    public virtual void Interact(Player plrRef)
    {
        // Not interactable if inactive
        if (!active) return;

        // Disable if oneshot
        if (isOneShot) active = false;

        // Handle player ladder stuff
        if (isLadderArea) {
            plrRef.isAutoWalking = true;
            plrRef.autoWalkDestinationX = Position.X;
            plrRef.ToggleLadder();
        }
        
        // Start dialogue box if one is linked
        if (dialogueBox != null) {
            dialogueBox.Call("start", startID);
            if (lockPlayerMovement) plrRef.SetMovementLock(true);
            return;
        }

        // We can skip doing scene stuff if there isn't a scene to load
        if (scenePath == null) return;

        // This might halt the program if the scene hasn't loaded yet. We could possibly prevent
        // this by calling this function during a scene transition (if one exists)
        if (scene == null)
        {
            scene = (PackedScene)ResourceLoader.LoadThreadedGet(scenePath);
        }

        // Lock player movement (unlocking it falls on the minigame)
        plrRef.SetMovementLock(true);

        // Instance the scene, adjust ZIndex so it renders on top
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
            Globals.Instance.currentSpawnPoint = spawnPoint;
            Globals.Instance.spawnFacingLeft = spawnFacingLeft;
            GetTree().ChangeSceneToPacked(scene);
        }
    }
}
