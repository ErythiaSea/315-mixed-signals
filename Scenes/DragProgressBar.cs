using Godot;
using System;

public partial class DragProgressBar : ProgressBar
{
	ShaderMaterial shaderMat;
	DraggableRect childDragger;

    [Export]
    float cursorDragRange = 80.0f;
    [Export]
    int BarDepletionRate = 15;


    private Vector2 StartPos;
    private Vector2 EndPos;
    public float startDragRange = 50.0f;
    public float progress;

    private bool hasStarted = false;
    private bool isDragging;
    private bool isCompleted = false;


    private Vector2 currentStart;
    private Vector2 currentEnd;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		shaderMat = Material.Duplicate() as ShaderMaterial;
		Material = shaderMat;

        StartPos = this.GlobalPosition;
        EndPos = StartPos + new Vector2(this.Size.X * this.Scale.X, 0).Rotated(this.Rotation);
    
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

        if (progress >= 0.99f)
        {
            isCompleted = true;
        }

        //if the drag is left without it being completed it begins reseting
        if (!isDragging && Value > 0 && !isCompleted)
        {
            ProgressReset(delta);
        }
    }

	public void MouseInput(InputEvent @event)
	{
        if (!isCompleted)
        {

            //checks if the event was a mouse input
            if (@event is InputEventMouseButton mouseButton)
            {
                //isdragging is based on the mouse input being left click
                isDragging = mouseButton.ButtonIndex == MouseButton.Left;

                if (StartPos.DistanceTo(mouseButton.GlobalPosition) < EndPos.DistanceTo(mouseButton.GlobalPosition))
                {
                    if((StartPos.DistanceTo(mouseButton.GlobalPosition) < startDragRange) && !hasStarted)
                    {
                        GD.Print("Started drag from left");
                        this.FillMode = 0;
                        currentStart  = StartPos;
                        currentEnd = EndPos;
                        shaderMat.SetShaderParameter("isLeft", true);
                        hasStarted = true;
                    }
                }
                else
                {
                    if ((EndPos.DistanceTo(mouseButton.GlobalPosition) < startDragRange) && !hasStarted)
                    {
                        GD.Print("Started drag from Right");
                        this.FillMode = 1;
                        currentStart = EndPos;
                        currentEnd = StartPos;
                        shaderMat.SetShaderParameter("isLeft", false);
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
            if (@event is InputEventMouseMotion mouseMotion && isDragging)
            {
 
                //progress is the dist of the mouse position along the path between start and end positions of the progress bar
                // returned value is clamped between 0 and 1 for lerp
                progress = Mathf.Clamp(ProgressAlongBar(mouseMotion.GlobalPosition), 0.0f, 1.0f);

                //updates progress bar
                Value = progress * MaxValue;

                if (progress > 0)
                {
                    float progressVal = (float)Value / (float)MaxValue;
                    shaderMat.SetShaderParameter("progress", progressVal);
                }


                //checks if within range so you cant drag from anywhere on screen
                //TODO: FIX THIS 
                //if (!IsWithinDragRange(mouseMotion.GlobalPosition))
                //{
                //    GD.Print("LeavingDragArea");
                //    isDragging = false;
                //}
            }
        }
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
        float dist = GlobalPosition.DistanceTo(MousePos);

        if (dist <= cursorDragRange)
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
        Value -= (BarDepletionRate * delta);

        //this.GlobalPosition = currentStart.Lerp(currentEnd, (float)Value / (float)MaxValue);

        if(Value < 20.0f)
        {
            hasStarted = false;
        }
    }
}
