using System;
using System.Collections.Generic;
using Godot;

public enum INPUT_METHODS
{
    KEYBOARD_CONTROLLER,
    KEYBOARD_ONLY,
    CONTROLLER_ONLY
}

public enum GAMEPAD
{
    XBOX = 0,
    PS = 1,
    NINTENDO = 2,
    KEYBOARD = 3,
    OTHER = 4
}

public struct InputStruct
{
    public string glyphName; 
    public String inputText;
    public INPUT_METHODS inputMethod = INPUT_METHODS.KEYBOARD_CONTROLLER;

    public InputStruct(String glpyh, String inputDesc, INPUT_METHODS inputMethods)
    {
        glyphName = glpyh;
        inputText = inputDesc;
        inputMethod = inputMethods;
    }
}

public partial class InputManager : Node
{
    // node instance for ready and input events
    public static InputManager Instance;

    // controller bool property and backing field
    private static bool _isController = false;
    public static bool IsController { 
        get { return _isController; }
        set
        {
            if (_isController == value) return;
            _isController = value;
            Globals.UpdateControlsText();
        }
    }

    // controller type property and backing field
    private static GAMEPAD _controllerType; // includes keyboard
    public static GAMEPAD ControllerType
    {
        get { return _controllerType; }
        set
        {
            if (_controllerType == value) return;
            _controllerType = value;

            // If our controller type changes we need to update
            // this text to reflect the new glyphs
            Globals.UpdateControlsText();
        }
    }

    private static bool _confirmCancelSwapped = false;
    public static bool ConfirmCancelSwapped { 
        get { return _confirmCancelSwapped; }
        set
        {
            if (_confirmCancelSwapped == value) return;
            _confirmCancelSwapped = value;

            Instance.SwapConfirmCancel(value);
        }
    }

	private void SwapConfirmCancel(bool on)
	{
        InputEventJoypadButton psX, psO;
        psX = new();
        psX.ButtonIndex = JoyButton.A;
        psO = new();
        psO.ButtonIndex = JoyButton.B;

        if (on)
        {
            InputMap.ActionAddEvent("ui_accept", psO);
            InputMap.ActionEraseEvent("ui_accept", psX);

            InputMap.ActionAddEvent("ui_cancel", psX);
            InputMap.ActionEraseEvent("ui_cancel", psO);

            InputMap.ActionAddEvent("interact", psO);
            InputMap.ActionEraseEvent("interact", psX);
        }
        else
        {
			InputMap.ActionAddEvent("ui_accept", psX);
			InputMap.ActionEraseEvent("ui_accept", psO);

			InputMap.ActionAddEvent("ui_cancel", psO);
			InputMap.ActionEraseEvent("ui_cancel", psX);

			InputMap.ActionAddEvent("interact", psX);
			InputMap.ActionEraseEvent("interact", psO);
		}
        GD.Print("a/b successfully swapped!");
        Globals.UpdateControlsText();
	}

	// there must be a better way, but i don't know it!
	// horizontal = a/d or left/right, vertical = w/s or up/down, all = wasd/dpad

	public static readonly Dictionary<GAMESTATE, List<InputStruct>> StateInputDict = new()
    {
        { GAMESTATE.MENU, new() {
            new InputStruct("vertical", "Select", INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("confirm", "Confirm", INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("cancel", "Back", INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.CUTSCENE, new() {
        } },
        { GAMESTATE.OVERWORLD, new() {
            new InputStruct("horizontal", "Move", INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("confirm", "Interact", INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.TRANSPOND, new() {
        } },
        { GAMESTATE.WAVEFORM, new() {
        } },
        { GAMESTATE.MAP, new() {
        } },
		{ GAMESTATE.CONSTELLATION, new() {
		} },
        { GAMESTATE.TRANSLATION, new() {
        } },
        { GAMESTATE.DIALOGUE, new() {
        } },
        { GAMESTATE.PHOTOBOARD, new() {
        } },
        { GAMESTATE.NONE, new() {
        } },
    };

    public override void _Ready()
    {
        // enforce singleton, only one should exist at a time
        if (Instance != null)
        {
            QueueFree();
        }
        Instance = this;

        // set controller type on game start based on initially connected pad (if any)
        // this can be changed later from settings
        _controllerType = GAMEPAD.PS;
        if (Input.GetConnectedJoypads().Count > 0)
        {
            IsController = true;
			string joyName = Input.GetJoyName(Input.GetConnectedJoypads()[0]);

			switch (joyName[0])
			{
				case 'P':
					ControllerType = GAMEPAD.PS;
					break;
				case 'X':
					ControllerType = GAMEPAD.XBOX;
					break;
				case 'N':
					ControllerType = GAMEPAD.NINTENDO;
                    ConfirmCancelSwapped = true;
					break;
				default:
					ControllerType = GAMEPAD.PS;
					break;
			}
		}
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion) return;
        if (@event is InputEventKey || @event is InputEventMouseButton)
        {
            IsController = false;
            //ControllerType = GAMEPAD.KEYBOARD;
        }
        else if (@event is InputEventJoypadButton || (@event is InputEventJoypadMotion stickMotion && Mathf.Abs(stickMotion.AxisValue) > 0.25))
        {
            IsController = true;
        }
       // GD.Print("Controller: " + IsController);
    }
}
