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
    Area2D GestureNode;
	Node2D LineParent;
	Node TreeRoot;
	PointLight2D Light;
	CircleShape2D StarShape;
	CollisionShape2D starCollision;
	Label CodeNumber;

	bool isDisplayed = false;

	//Array for all lines within the constellation telescope minigame
	Godot.Collections.Array<Node> Lines;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Number display and star light up node references
		CodeNumber = GetNode<Label>("CodeNumber");
		Light = GetNode<PointLight2D>("StarLight");

		//Reference to Root node of tree and through it the gesture node
        TreeRoot = GetTree().Root.GetChild(1);
        GestureNode = TreeRoot.FindChild("Gesture", true, false) as Area2D;

        //Reference to the StarCollison and its shape for checking if lines have overlapped over its area
        StarShape = GetNode<CollisionShape2D>("StarCollision").Shape as CircleShape2D;
        starCollision = GetNode<CollisionShape2D>("StarCollision");

        //both set to false for visibiliy, only shows when constellation done succesfully
        CodeNumber.Visible = false;
        Light.Visible = false;


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
		if (!isDisplayed)
		{
            RandomNumber();
            CodeNumber.Visible = true;
			isDisplayed = true;
        }
    }

	void RandomNumber()
	{
		var rng = new RandomNumberGenerator();

		int randomNum = rng.RandiRange(-4, 4);
		CodeNumber.Text = string.Empty;
		CodeNumber.Text = randomNum.ToString();
	}
}
