using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;
using System.Runtime;

public partial class DragProgressBar : ProgressBar
{
	ShaderMaterial shaderMat;

    [Export]
    float beginDragRange = 30.0f;
    [Export]
    int barDepletionRate = 15;

    private static DragProgressBar currentBar = null;

    //Public bar positions
    public Vector2 startPos;
    public Vector2 endPos;
    public float progress;

    //progress states
    private bool hasStarted = false;
    public bool isCompleted = false;

    //Mouse positions
    Vector2 mouseDir;
    Vector2 prevMousePos;
    Vector2 mousePos;
    Godot.Collections.Array<Node> sceneBars;
 
    //Private bar positions
    private Vector2 progressVec;
    private Vector2 currentStart;
    private Vector2 currentEnd;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		shaderMat = Material.Duplicate() as ShaderMaterial;
		Material = shaderMat;

        startPos = this.GlobalPosition;

        Node2D dragRoot = this.GetParent().GetParent() as Node2D;
        endPos = startPos + new Vector2(this.Size.X * (this.Scale.X * dragRoot.Scale.X),0).Rotated(this.Rotation);

        sceneBars = this.GetParent().GetChildren(false);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

        //mouse positions and direction updates
        mousePos = GetGlobalMousePosition();
        mouseDir = (mousePos - prevMousePos).Normalized();

        prevMousePos = mousePos;

        //if(isDragging && this.GetRect().HasPoint(GetGlobalMousePosition()))
        //{
        //    //Have drag affect
        //    //probably make the affect a child of the top layer canvas of all the progress bars
        //

        //updates the bars progress when its the current bar
        if (currentBar == this && !isCompleted)
        {
            progress = Mathf.Clamp(ProgressAlongBar(), 0.0f, 1.0f);
            progressVec = currentStart.Lerp(currentEnd, progress);
            Value = progress * MaxValue;

            //when completed check for adjacent bars
            if (progress >= 0.99f)
            {
                isCompleted = true;
  
                if (sceneBars.Count > 0)
                {
                    CheckAdjacentBars();
                }
            }
        }
        //if the drag is left without it being completed it begins reseting
        if (currentBar != this && Value > 0 && !isCompleted)
        {
            ProgressReset(delta);
        }

        if (currentBar == this && !IsWithinDragRange())
        {
            currentBar = null;
        }
    }

    public void MouseInput(InputEvent @event)
    {
        //mouse click to initatiate the drag
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            GD.Print("Clicked");
            //current bar is now this bar
            currentBar = this;

            //checks which end of the bar the mouse click is closer too to find out which direction to drag
            if (startPos.DistanceTo(mousePos) < endPos.DistanceTo(mousePos))
            {
                //checks if the clicks position is within the range to actually start a drag and that it isnt already dragging
                if ((startPos.DistanceTo(mousePos) < beginDragRange) && !hasStarted)
                {
                    DragSettings(0);
                }
            }
            else
            {
                if ((endPos.DistanceTo(mousePos) < beginDragRange) && !hasStarted)
                {
                    DragSettings(1);
                }
            }
            //if released at all when dragging, no longer current bar
            if (mouseButton.IsReleased())
            {
                currentBar = null;
            }
        }
    }

    //function for setting up the core dragging settings 
    public void DragSettings(int fillmode)
    {
        if (fillmode == 0)
        {
            FillMode = 0;
            currentStart = startPos;
            currentEnd = endPos;
            progressVec = currentStart;
            hasStarted = true;
            Value = 0;
        }
        else
        {
            FillMode = 1;
            currentStart = endPos;
            currentEnd = startPos;
            progressVec = currentStart;
            hasStarted = true;
            Value = 0;
        }
    }

    //function for calculating the distance of the mousePos along the path between endPos and StartPos
    //Projects mousePos along path to find the distance and progress along the path at that time
    private float ProgressAlongBar()
    {
        Vector2 PathVec = currentEnd - currentStart;
        Vector2 MouseVec = mousePos - currentStart;

        float dist = MouseVec.Project(PathVec).Length() / PathVec.Length();

        return dist;
    }

    //checks if the mouse position is within cursorDragRange
    private bool IsWithinDragRange()
    {
  
        float dist = mousePos.DistanceTo(progressVec);

        if (dist <= beginDragRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //decreases the progress value and pulls the draggable object back to the start position 
    private void ProgressReset(double delta)
    {
        Value -= (barDepletionRate * delta);

        progressVec = currentStart.Lerp(currentEnd, (float)Value / (float)MaxValue);

        if(Value < 20.0f)
        {
            hasStarted = false;
        }
    }

    private void CheckAdjacentBars()
    {
        DragProgressBar targetBar = new DragProgressBar();
        DragProgressBar currentBarIndex;
        int fm = 0;
        float bestDot = 0;

        for (int i = 0; i < sceneBars.Count; i++)
        {

            currentBarIndex = sceneBars[i] as DragProgressBar;

            if (currentBarIndex != this && !currentBarIndex.isCompleted)
            {
                if (currentBar == this && currentBarIndex.endPos.DistanceTo(currentEnd) < beginDragRange || currentBarIndex.startPos.DistanceTo(currentEnd) < beginDragRange)
                {
                    Vector2 barDir = new Vector2();
                    //gets the bar direction based on how close the start of the bar is to the side closet to this bar
                    if (currentBarIndex.startPos.DistanceTo(currentEnd) < currentBarIndex.endPos.DistanceTo(currentEnd))
                    {
                        barDir = (currentBarIndex.endPos - currentBarIndex.startPos);
                        fm = 0;
                    }
                    else
                    {
                        barDir = (currentBarIndex.startPos - currentBarIndex.endPos);
                        fm = 1;
                    }

                    //compares mouse direction with bar direction
                    float dotProd = mouseDir.Dot(barDir);

                    //closest match is the new bar to continue dragging into
                    if (dotProd > bestDot)
                    {
                        bestDot = dotProd;
                        targetBar = currentBarIndex;
                    }
                }
            }
        }

        //update the current bar to the new bar and set it up for dragging
        targetBar.DragSettings(fm);
        currentBar = targetBar;
    }
}
