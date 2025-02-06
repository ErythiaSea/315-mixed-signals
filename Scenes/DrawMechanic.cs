using Godot;
using System;
using System.ComponentModel.Design;
public partial class DrawMechanic : CollisionShape2D
{
	GodotObject GestureScript;
    Area2D GestureNode;
    Marker2D ToggleOffPos;
    Marker2D ToggleOnPos;

    bool isToggled = false;
    bool isDoneMoving = true;
    	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //Gets off and on marker nodes for toggle positions
        ToggleOffPos = this.GetNode<Marker2D>("../ToggleOff");
        ToggleOnPos = this.GetNode<Marker2D>("../ToggleOn");

        GestureNode = this.GetParent<Area2D>();
        //Inital position of canvas will be toggled off
        this.Position = ToggleOffPos.Position;

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
            isToggled = !isToggled;
            isDoneMoving = false;
        }

        if (!isDoneMoving)
        {
            CanvasToggle(delta);
        }

        GD.Print(GestureNode.Position);
    }

    public void draw()
    {
        RectangleShape2D shp = this.Shape as RectangleShape2D;
        //new Rect2(this.Position, shp.Size)
        DrawRect(this.Shape.GetRect(), Colors.Black, false, 1.0f, false);
    }

    public void CanvasToggle(double delta)
    {
        float InterpSpeed = (float)delta * 0.9f; 
        //base this off of gesture position instead
        if (isToggled)
        {
            if (!this.Position.IsEqualApprox(ToggleOnPos.Position))
            {
                
                this.Position = this.Position.Lerp(ToggleOnPos.Position, InterpSpeed);
            }
            else
            {
                isDoneMoving = true;
            }
        }
        else if (!isToggled)
        {
            if (!this.Position.IsEqualApprox(ToggleOffPos.Position))
            {
                this.Position = this.Position.Lerp(ToggleOffPos.Position, InterpSpeed);
            }
            else
            {
                isDoneMoving = true;
            }
        }
    }

}
