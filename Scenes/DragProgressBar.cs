using Godot;
using Godot.NativeInterop;
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

    private static DragProgressBar CurrentBar = null;

    public Vector2 StartPos;
    public Vector2 EndPos;
    public float startDragRange = 40.0f;
    public float progress;

    private bool hasStarted = false;
    private bool isCompleted = false;
    Vector2 mousePos;
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

        Node SceneRoot = GetTree().Root.GetChild(0);
        SceneBars = SceneRoot.GetChild(0).GetChildren(false);

       
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        mousePos = GetGlobalMousePosition();
        

        //if(isDragging && this.GetRect().HasPoint(GetGlobalMousePosition()))
        //{
        //    //Have drag affect
        //    //probably make the affect a child of the top layer canvas of all the progress bars
        //

        if (CurrentBar == this && !isCompleted)
        {
            progress = Mathf.Clamp(ProgressAlongBar(), 0.0f, 1.0f);
            progressVec = currentStart.Lerp(currentEnd, progress);
            Value = progress * MaxValue;

            if (progress >= 0.99f)
            {
                isCompleted = true;
                GD.Print("barcount: " + SceneBars.Count);
                if (SceneBars.Count > 0)
                {
                    CheckAdjacentBars();
                }
            }
        }

        //if the drag is left without it being completed it begins reseting
        if (CurrentBar != this && Value > 0 && !isCompleted)
        {
            ProgressReset(delta);
        }

        if (CurrentBar == this && !IsWithinDragRange())
        {
            CurrentBar = null;
        }
    }

    public void MouseInput(InputEvent @event)
    {
        GD.Print(this.GetInstanceId());

        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            GD.Print("id:" + this.GetInstanceId());
            CurrentBar = this;

            if (StartPos.DistanceTo(mouseButton.GlobalPosition) < EndPos.DistanceTo(mouseButton.GlobalPosition))
            {
                if ((StartPos.DistanceTo(mouseButton.GlobalPosition) < startDragRange) && !hasStarted)
                {
                    DragSettings(0);
                }
            }
            else
            {
                if ((EndPos.DistanceTo(mouseButton.GlobalPosition) < startDragRange) && !hasStarted)
                {
                    DragSettings(1);
                }
            }
            //if released at all when isdragging is false
            if (mouseButton.IsReleased())
            {
                GD.Print("real");
                CurrentBar = null;
            }
        }
    }

    public void DragSettings(int fillmode)
    {
        GD.Print("continue drag");

        if (fillmode == 0)
        {
            FillMode = 0;
            currentStart = StartPos;
            currentEnd = EndPos;
            progressVec = currentStart;
            hasStarted = true;
            Value = 0;
        }
        else
        {
            FillMode = 1;
            currentStart = EndPos;
            currentEnd = StartPos;
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
            GD.Print("BARINDEX: " + i);
            GD.Print("NAMEOF: " + SceneBars[i]);
            currentBarIndex = SceneBars[i] as DragProgressBar;
            GD.Print("NAMEOF: " + currentBarIndex.Name);
            if(currentBarIndex != this)
            {
                GD.Print("currentbarIndex is not currentBar");

                if (CurrentBar == this && currentBarIndex.EndPos.DistanceTo(currentEnd) < 10 || currentBarIndex.StartPos.DistanceTo(currentEnd) < 10)
                {
                    GD.Print("Valid adjacent bar");
                    if (isCompleted)
                    {
                        GD.Print("CHANGE OVER");
                        currentBarIndex.DragSettings(this.FillMode);
                        CurrentBar = currentBarIndex;
                    }

                }
            }
        }
    }
}
