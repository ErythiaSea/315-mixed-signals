using Godot;
using System;
using System.Linq;

public partial class DragProgressBar : ProgressBar
{
	ShaderMaterial shaderMat;
	DraggableRect childDragger;

    [Export]
    float cursorDragRange = 20.0f;
    [Export]
    int BarDepletionRate = 15;

    public bool isContinued;

    public Vector2 StartPos;
    public Vector2 EndPos;
    public float startDragRange = 40.0f;
    public float progress;

    private bool hasStarted = false;
    public bool isDragging;
    private bool isCompleted = false;

    Godot.Collections.Array<Node> SceneBars;
    public InputEvent inputev;
    private Vector2 progressVec;
    private Vector2 currentStart;
    private Vector2 currentEnd;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		shaderMat = Material.Duplicate() as ShaderMaterial;
		Material = shaderMat;

        StartPos = this.GlobalPosition;
        EndPos = StartPos + new Vector2(this.Size.X * this.Scale.X,0).Rotated(this.Rotation);

        Node root = GetTree().Root.GetChild(0);

        SceneBars = this.GetParent().GetChildren(false);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        GD.Print("PROG: " + progressVec);
        if (progress >= 0.99f)
        {
            isCompleted = true;

            if(SceneBars.Count > 0)
            {
                CheckAdjacentBars();
            }
        }

        //if the drag is left without it being completed it begins reseting
        if (!isDragging && Value > 0 && !isCompleted)
        {
            ProgressReset(delta);
        }
    }

	public void MouseInput(InputEvent @event)
	{
        GD.Print(this.GetInstanceId());
        if (!isCompleted)
        {
            GD.Print("is not completed");
            //checks if the event was a mouse input
            if (@event is InputEventMouseButton mouseButton)
            {
                GD.Print("is a mouse button");
                //isdragging is based on the mouse input being left click
                isDragging = mouseButton.ButtonIndex == MouseButton.Left;

                if (StartPos.DistanceTo(mouseButton.GlobalPosition) < EndPos.DistanceTo(mouseButton.GlobalPosition))
                {
                    if((StartPos.DistanceTo(mouseButton.GlobalPosition) < startDragRange) && !hasStarted)
                    {
                        this.Value = 0;
                        this.FillMode = 0;
                        currentStart  = StartPos;
                        currentEnd = EndPos;
                        progressVec = currentStart;
                        hasStarted = true;
                    }
                }
                else
                {
                    if ((EndPos.DistanceTo(mouseButton.GlobalPosition) < startDragRange) && !hasStarted)
                    {
                        this.Value = 0;
                        this.FillMode = 1;
                        currentStart = EndPos;
                        currentEnd = StartPos;
                        progressVec = currentStart;
                        hasStarted = true;
                    }
                }
                //if released at all when isdragging is false
                if (mouseButton.IsReleased())
                {
                    isDragging = false;
                }
            }


            //if there is mouse motion while dragging then drag the object for progress
            if (@event is InputEventMouseMotion mouseMotion && isDragging && IsWithinDragRange(mouseMotion.GlobalPosition))
            {
                GD.Print("is mouse motion");
                //progress is the dist of the mouse position along the path between start and end positions of the progress bar
                // returned value is clamped between 0 and 1 for lerp
                progress = Mathf.Clamp(ProgressAlongBar(mouseMotion.GlobalPosition), 0.0f, 1.0f);

                //progress vector for keeping the mouse in range of the drag
                progressVec = currentStart.Lerp(currentEnd, progress);
                
                //updates progress bar
                Value = progress * MaxValue;

                if (progress > 0)
                {
                    float progressVal = (float)Value / (float)MaxValue;
                    shaderMat.SetShaderParameter("progress", progressVal);
                }
            }


        }
    }

    public void MouseEnteredSpace()
    {

    }
    //function for calculating the distance of the mousePos along the path between endPos and StartPos
    //Projects mousePos along path to find the distance and progress along the path at that time
    private float ProgressAlongBar(Vector2 MousePos)
    {
        Vector2 PathVec = currentEnd - currentStart;
        Vector2 MouseVec = MousePos - currentStart;

        float dist = MouseVec.Project(PathVec).Length() / PathVec.Length();

        return dist;
    }

    //checks if the mouse position is within cursorDragRange
    private bool IsWithinDragRange(Vector2 MousePos)
    {
  
        float dist = MousePos.DistanceTo(progressVec);
        GD.Print("vec1: " + progressVec);
        GD.Print("vec2: " + MousePos);
        if (dist <= cursorDragRange)
        {
            GD.Print("within range");
            return true;
        }
        else
        {
            GD.Print("Not within range");
            return false;
        }
    }

    //decreases the progress value and pulls the draggable object back to the start position 
    private void ProgressReset(double delta)
    {
        Value -= (BarDepletionRate * delta);

        progressVec = currentStart.Lerp(currentEnd, (float)Value / (float)MaxValue);

        if(Value < 20.0f)
        {
            hasStarted = false;
        }
    }

    private void CheckAdjacentBars()
    {
        DragProgressBar currentBarIndex;
        for (int i = 0; i < SceneBars.Count; i++)
        {
            currentBarIndex = SceneBars[i] as DragProgressBar;

            if ((isDragging) && currentBarIndex.EndPos.DistanceTo(currentEnd) < 10 || currentBarIndex.StartPos.DistanceTo(currentEnd) < 10)
            {
                GD.Print("Valid adjacent bar");
                if (isCompleted) 
                {
                
                }

            }
        }
    }
}
