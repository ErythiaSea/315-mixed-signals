using Godot;
using System;
using System.Drawing;
using System.Linq;

public partial class BorderDraw : Control
{
	// Called when the node enters the scene tree for the first time.
	CanvasLayer childCanvas;
    Godot.Collections.Array<Node> containerNodes;
    public override void _Ready()
	{
		childCanvas = this.GetChild(0) as CanvasLayer;

        //containerNodes = childCanvas.GetChildren(true);
        //GetAllChildren();
        //DrawContainers();


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("close")) { Close(); }
	}

    private void GetAllChildren()
    {
        foreach(Node ctrl in childCanvas.GetChildren())
        {
            if(ctrl.GetChildCount() > 0)
            {
                containerNodes.Append(ctrl);
                foreach (Node child in ctrl.GetChildren())
                {
                    if(child.GetChildCount() > 0)
                    {
                        containerNodes.Append(child);
                        containerNodes += child.GetChildren(false);
                    }
                    else
                    {
                        containerNodes.Append(child);
                    }
                }
            }
            else
            {
                containerNodes.Append(ctrl);
            }
        }

        GD.Print("Size: " + containerNodes.Count);
    }
    private void DrawContainers()
    {
        Control currentContainerIndex = null;
        for (int i = 0; i < containerNodes.Count; i++)
        {
            currentContainerIndex = containerNodes[i] as Control;

            
            DrawRect(currentContainerIndex.GetRect(), new Godot.Color(0,0,0,1), false);
        }
    }

    void Close()
    {
        Player plr = GetNode<Player>("../Player");
        if (plr != null)
        {
            plr.setMovementState(MovementStates.FREE_MOVE);
            QueueFree();
        }
    }
}
