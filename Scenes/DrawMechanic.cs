using Godot;
using System;
using System.ComponentModel.Design;
public partial class DrawMechanic : CollisionShape2D
{
	GodotObject GestureScript;
    Area2D GestureNode;
    Marker2D ToggleOffPos;
    Marker2D ToggleOnPos;

    public bool isToggled = false;
    public bool isDoneMoving = true;
    	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GestureNode = this.GetParent<Area2D>();

        //Gets off and on marker nodes for toggle positions
        ToggleOffPos = GestureNode.GetNode<Marker2D>("../ToggleOff");
        ToggleOnPos = GestureNode.GetNode<Marker2D>("../ToggleOn");
      

        //Inital position of canvas will be toggled off
        GestureNode.Position = ToggleOffPos.Position;

        //Loads the GDscript to allow use of it within c# in this script
        var Gesture = GD.Load<GDScript>("res://addons/gesture_recognizer/scripts/ControlGesture.gd");
        GestureScript = (GodotObject)Gesture.New();

        //calls the gesture classify function within the ControlGesture script
        GestureScript.Set("ClassifyGesture", true);

        //calls draw function linked to drawing signal, to draw the outline of collisionshape2D
        draw();

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

        if (Input.IsActionJustPressed("ToggleNotebook"))
        {
            GD.Print("Moving has started");
            isToggled = !isToggled;
            isDoneMoving = false;
        }

        if (!isDoneMoving)
        {
            CanvasToggle(delta);
        }

        //if(GetNode("/root/Line") != null)
        //{
        //    GD.Print("Lines exist");
        //}
        //GD.Print(GestureNode.Position);
    }

    public void draw()
    {
        RectangleShape2D shp = this.Shape as RectangleShape2D;
        //new Rect2(this.Position, shp.Size)
        DrawRect(this.Shape.GetRect(), Colors.Black, false, 1.0f, false);
    }

    public void CanvasToggle(double delta)
    {
        float InterpSpeed = (float)delta * 10f; 
        //base this off of gesture position instead
        if (isToggled)
        {
            if (Mathf.Abs(GestureNode.Position.Y - ToggleOnPos.Position.Y) > 0.5f)
            {
                
                GestureNode.Position = GestureNode.Position.Lerp(ToggleOnPos.Position, InterpSpeed);
            }
            else
            {
                GD.Print("Moving has stopped");
                isDoneMoving = true;
            }
        }
        else if (!isToggled)
        {
            if (Mathf.Abs(GestureNode.Position.Y - ToggleOffPos.Position.Y) > 0.5f)
            {
                GestureNode.Position = GestureNode.Position.Lerp(ToggleOffPos.Position, InterpSpeed);
            }
            else
            {
                GD.Print("Moving has stopped");
                isDoneMoving = true;
            }
        }
    }

}
