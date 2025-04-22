using System;
using System.Collections.Generic;
using Godot;

public enum INPUT_METHODS
{
    KEYBOARD_CONTROLLER,
    KEYBOARD_ONLY,
    CONTROLLER_ONLY
}

public struct InputStruct
{
    public List<string> glyphName; 
    public String inputText;
    public int priority;
    public INPUT_METHODS controllerOnly = INPUT_METHODS.KEYBOARD_CONTROLLER;

    public InputStruct()
    {
        glyphName = new List<string>();
        inputText = string.Empty;
        priority = 0;
        controllerOnly = INPUT_METHODS.KEYBOARD_CONTROLLER;
    }

    public InputStruct(String glpyh, String inputDesc, int orderPriority, INPUT_METHODS inputMethods)
    {
        glyphName = new List<string>() { glpyh };
        inputText = inputDesc;
        priority = orderPriority;
        controllerOnly = inputMethods;
    }
}

public static class InputData
{
    // there must be a better way, but i don't know it!
    // horizontal = a/d or left/right, vertical = w/s or up/down, all = wasd/dpad
    // otherwise 
    public static readonly Dictionary<GAMESTATE, List<InputStruct>> StateInputDict = new()
    {
        { GAMESTATE.MENU, new() {
            new InputStruct("vertical", "Up", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("confirm", "Confirm", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("cancel", "Back", 0, INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.CUTSCENE, new() {
        } },
        { GAMESTATE.OVERWORLD, new() {
            new InputStruct("horizontal", "Move", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("confirm", "Interact", 0, INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.TRANSPOND, new() {
            new InputStruct("ui_up", "transpond", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_down", "Down", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_accept", "Confirm", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_cancel", "Back", 0, INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.WAVEFORM, new() {
            new InputStruct("ui_up", "waveform", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_down", "Down", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_accept", "Confirm", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_cancel", "Back", 0, INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.MAP, new() {
            new InputStruct("ui_up", "Up", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_down", "Down", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_accept", "Confirm", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_cancel", "Back", 0, INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.TRANSLATION, new() {
            new InputStruct("ui_up", "Up", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_down", "Down", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_accept", "Confirm", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_cancel", "Back", 0, INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.DIALOGUE, new() {
            new InputStruct("ui_up", "Up", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_down", "Down", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_accept", "Confirm", 0, INPUT_METHODS.KEYBOARD_CONTROLLER),
            new InputStruct("ui_cancel", "Back", 0, INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.PHOTOBOARD, new() {
            new InputStruct("ui_cancel", "Back", 0, INPUT_METHODS.KEYBOARD_CONTROLLER)
        } },
        { GAMESTATE.NONE, new() {
        } },
    };
}
