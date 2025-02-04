using Godot;
using System;
public partial class DrawMechanic : CollisionShape2D
{
	GodotObject GestureNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

        var Gesture = GD.Load<GDScript>("res://addons/gesture_recognizer/scripts/ControlGesture.gd");
        GestureNode = (GodotObject)Gesture.New();

        if (GestureNode != null)
        {
            GD.Print("no null");
        }
        else
        {
            GD.Print("null");
        }

        GestureNode.Set("ClassifyGesture", true);

       // DrawRect(this.Shape.GetRect(), Colors.Black, false, 1.0f, false);

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

}
