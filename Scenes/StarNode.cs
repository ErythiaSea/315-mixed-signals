using Godot;
using Godot.NativeInterop;
using System;
using System.ComponentModel;

//public partial class ControlGesture : Area2D
//{
//	public static ControlGesture Instance { get;private set; }
//	public bool isComplete { get; set; }

//    public override void _Ready()
//    {
//        Instance = this;
//    }

public partial class StarNode : Area2D
{
    //Objects
    GodotObject GestureScript;
    Area2D GestureNode;
	Node2D LineParent;
	Node TreeRoot;
	PointLight2D Light;
	CircleShape2D StarShape;
	CollisionShape2D starCollision;
	Label CodeNumber;

	Godot.Collections.Array<Node> Lines;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CodeNumber = GetNode<Label>("CodeNumber");
		CodeNumber.Visible = false;
		Light = GetNode<PointLight2D>("StarLight");
		Light.Visible = false;
		TreeRoot = GetTree().Root.GetNode("ConstellationDraw");

		StarShape = GetNode<CollisionShape2D>("StarCollision").Shape as CircleShape2D;
		starCollision = GetNode<CollisionShape2D>("StarCollision");

        var Gesture = GD.Load<GDScript>("res://addons/gesture_recognizer/scripts/ControlGesture.gd");
        GestureScript = (GodotObject)Gesture.New();

        GestureNode = TreeRoot.FindChild("Gesture", true, false) as Area2D;

		if (GestureNode == null)
		{
			GD.Print("Gesture null");
		}
		//_Draw();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{


		//if(GestureNode.FindChild("Line",false,false) != null)
		//{
		//	LineParent = GestureNode.FindChild("Line", false, false) as Node2D;
		//}

		//if (LineParent != null && LineParent.GetChildCount() > 0)
		//{
		//	GD.Print("passed");

		//	Lines = LineParent.GetChildren();

		//	for (int i = 0; i < Lines.Count; i++)
		//	{
		//		//GD.Print("Checking lines, lineCount: " + Lines.Count);

		//		if(line_Overlapped(Lines[i] as Line2D))
		//		{
		//			//if this passes once then leave the for loop for this frame
		//			GD.Print("Line overlapped Star");
		//			Light.Visible = true;
		//		}
		//	}
		//}

	}

    //public override void _Draw()
    //{

    //    DrawCircle(starCollision.Position,StarShape.Radius, Colors.White, true, 1.0f, true);
    //}

	void Area_Entered(Node2D body)
	{
		GD.Print("Line2D Detected");
	}

	//Change shape to rect2 for haspoint func and use that instead, way easier, way less hassle
	bool line_Overlapped(Line2D line)
	{
		Vector2[] linePoints = line.Points;

		for(int i = 0; i < linePoints.Length; i++)
		{
			//GD.Print("node pos: " + ToGlobal(this.Position) + " line point: " + ToGlobal(linePoints[i]));
			if (ToGlobal(this.Position).DistanceTo(ToGlobal(linePoints[i])) < StarShape.Radius)
			{
				GD.Print("lineOverlap true");
				return true;
			}
		}
		GD.Print("LineOverlap false");
		return false;
	}

	void DisplayCodeNumbers()
	{
        GD.Print("easy");
        CodeNumber.Visible = true;
    }
}
