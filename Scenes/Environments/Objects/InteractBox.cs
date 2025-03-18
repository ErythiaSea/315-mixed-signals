using Godot;
using System;
using System.ComponentModel.Design;


public enum TRANSITION
{
    NONE,
    LEFTtoRIGHT,
    RIGHTtoLEFT,
    TOPtoBOTTOM,
    BOTTOMtoTOP
}
public partial class InteractBox : Area2D
{
    // whether the interact box can be used
    [Export]
    public bool active = true;

    // the item that will be highlighted in the overworld when the player is in this interact box
    [Export]
    public CanvasItem objectSprite;
    ShaderMaterial sMaterial;

    // if the item should use a custom outline color
    [Export]
    bool useCustomOutlineColor = false;

    // the custom outline color to be used
    [Export]
    public Color customOutlineColor;

    [ExportGroup("Scene Loading")]
    // the scene to load (for minigames)
    //[Export]
    //PackedScene scene;

    //the type of transition
    [Export]
    public TRANSITION transitionType;

    [Export]
    public float transitionLength = 1.0f;

    //required stage to be able to interact
    [Export]
    public GAMESTAGE requiredStage;

    // temp for scenes that reference other scenes (usually through an object of this class)
    // to prevent circular dependencies. todo: make this suck less
    [Export(PropertyHint.File, "*.tscn")]
    public string scenePath = null;

    // the above scenePath once loaded will be stored here
    private PackedScene scene = null;

    // swap to this scene (level transition) or instanciate it in the current
    [Export]
    bool loadInCurrent = true;

    // the zero-indexed spawn point ID that the player will load in at (if this is a level transition)
    // if this is < 0 the player's default scene position will be used instead
    [Export]
    int spawnPoint = -1;

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
    Control dialogueBox;

    // the start id for the dialogue
    [Export]
    String startID;

    // how long the dialogue bubble should exist for
    [Export]
    float timeLimit = 0;

    // if the dialogue box should lock the player's movement
    [Export]
    bool lockPlayerMovement = false;

    float transitionTime = 0.0f;
    bool isTransition = false;
    private Player player;

    private Tween tween;
    public bool isPlayerInArea = false;
    float outlineAlpha = 0.0f;

    public override void _Ready()
    {
        // possible todo: for things that will have a transition before getting the loaded file, we can
        // move the load request to Interact() so that other levels aren't loaded in memory the whole time
        if (scenePath != null)
        {
            ResourceLoader.LoadThreadedRequest(scenePath);
        }

        if (objectSprite != null)
        {
            sMaterial = objectSprite.Material as ShaderMaterial;
        }
        if (sMaterial != null) 
        { 
            if (useCustomOutlineColor)
            {
                sMaterial.SetShaderParameter("line_color", new Vector3(customOutlineColor.R, customOutlineColor.G, customOutlineColor.B));
            }
            else
            {
                Vector3 col = Globals.STANDARD_OUTLINE_COLOR;
                sMaterial.SetShaderParameter("line_color", new Vector3(col.X, col.Y, col.Z));
            }
        }

        // connect events to functions
        AreaEntered += areaEntered;
        AreaExited += areaExited;

    }
    public override void _Process(double delta)
    {
        if (isTransition)
        {
            runningTransition(delta);
        }

        if (sMaterial != null)
        {
            sMaterial.SetShaderParameter("line_alpha", outlineAlpha);
        }
    }

    public virtual void Interact(Player plrRef)
    {
        player = plrRef;

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
            // set a time limit if this is a dialogue bubble
            if (timeLimit > 0 && (dialogueBox as RichTextLabel) != null)
            {
                dialogueBox.Set("time_limit", timeLimit);
            }
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

        // If this interact box has a transition assigned, do it
        // Otherwise, just load the scene immediately
		if (transitionType != TRANSITION.NONE)
        {
			isTransition = true;
			plrRef.EmitSignal("Transition", (int)transitionType, transitionLength);
		} 
        else { loadScene(); }
    }

    private bool IsCorrectStage()
    {
        if (requiredStage == GAMESTAGE.TRANSITION) return true;

        if (requiredStage != Globals.Instance.gameState.stage) return false;
        else return true;
    }

    private void runningTransition(double delta)
    {
		transitionTime += (float)delta;

		if (transitionTime > transitionLength)
        {
            isTransition = false;
            transitionTime = 0;
            loadScene();
        }
    }

    // Loads the scene, either as a child of the current scene, or swapping to it
    // based on the loadInCurrent bool
    private void loadScene()
    {
		// Instance the scene, adjust ZIndex so it renders on top
		if (loadInCurrent)
		{
			CanvasLayer instancedGame = (CanvasLayer)scene.Instantiate();

			GetParent().AddChild(instancedGame);
			instancedGame.Layer = 2;

            // Note: we probably shouldn't need to disable the player camera anymore (for minigames),
            // this field can probably be removed entirely - erf
            if (disablePlayerCam)
            {
                player.SetCameraEnabled(false);
            }
        }
        else
        {
            Globals.Instance.currentSpawnID = spawnPoint;
            GD.Print("loading new scene...");
            GetTree().ChangeSceneToPacked(scene);
		}
	}

    private void areaEntered(Area2D area)
    {
        // if it's the player's area
        if (area.GetParent<Player>() != null)
        {
            GD.Print("player entered area!");
            isPlayerInArea = true;
            if (sMaterial != null)
            {
                GD.Print("go");
                tween = CreateTween().SetLoops();
                outlineAlpha = 0.3f;
                tween.TweenProperty(this, "outlineAlpha", 1.0f, 0.75);
                tween.TweenProperty(this, "outlineAlpha", 0.3f, 0.75);
            }
        }
    }

    private void areaExited(Area2D area)
    {
        // if it's the player's area
        if (area.GetParent<Player>() != null)
        {
            isPlayerInArea = false;
            tween.Kill();
            outlineAlpha = 0;
        }
    }
}
