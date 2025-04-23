 using Godot;
using System;
using System.Drawing;
using System.Linq;

public partial class BorderDraw : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	CanvasLayer childCanvas;
	Godot.Collections.Array<Node> containerNodes;
	public override void _Ready()
	{
		childCanvas = this.GetChild(0) as CanvasLayer;

		//containerNodes = childCanvas.GetChildren(true);
		GetAllChildren(childCanvas);
		//DrawContainers();
		GD.Print(containerNodes.Count);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("close")) { Close(); }
	}

	private void GetAllChildren(Node node)
	{
			if (node.GetChildCount() > 0)
			{
				containerNodes.Add(node);
				GetAllChildren(node);
			}
			else
			{
				containerNodes.Add(node);
			}
			GD.Print("Size: " + containerNodes.Count);
	}
	private void DrawContainers()
	{
		Control currentContainerIndex = null;
		for (int i = 0; i < containerNodes.Count; i++)
		{
			currentContainerIndex = containerNodes[i] as Control;

			currentContainerIndex.DrawRect(currentContainerIndex.GetRect(), new Godot.Color(0,0,0,1), false);
		}
	}

	private void Close()
	{
		Player plr = GetNode<Player>("../Player");
		if (plr != null)
		{
			plr.SetMovementLock(false);
			QueueFree();
		}
	}
}
