using Godot;
using System;

public partial class DraggableRect : TextureRect
{
	[Export]
	float cursorDragRange = 50.0f;
	[Export]
	int BarDepletionRate = 5;


	private Vector2 StartPos;
	private Vector2 EndPos;

	public float progress;

	private bool isDragging;
	private bool isCompleted = false;

	ProgressBar progressBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		progressBar = this.GetParent<ProgressBar>();

		Connect("gui_input", new Callable(this, nameof(MouseInput)));
		//Start and end of progressbar
		StartPos = progressBar.GlobalPosition;
		EndPos = StartPos + new Vector2(progressBar.Size.X * progressBar.Scale.X, 0).Rotated(progressBar.Rotation);

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Completes this slider when almost fully dragged
		if (progress >= 0.99f)
		{
			isCompleted = true;
		}

		//if the drag is left without it being completed it begins reseting
		if (!isDragging && progressBar.Value > 0 && !isCompleted)
		{
			ProgressReset(delta);
		}
	
	}

	private void MouseInput(InputEvent @event)
	{
		GD.Print(progressBar.Name);
		if (!isCompleted)
		{

			//checks if the event was a mouse input
			if (@event is InputEventMouseButton mouseButton)
			{
				//isdragging is based on the mouse input being left click
				isDragging = mouseButton.ButtonIndex == MouseButton.Left;

				//if released at all when isdragging is false
				if (mouseButton.IsReleased())
				{
					isDragging = false;
				}
			}

			//if there is mouse motion while dragging then drag the object for progress
			if (@event is InputEventMouseMotion mouseMotion && isDragging)
			{
				GetViewport().SetInputAsHandled();
				//progress is the dist of the mouse position along the path between start and end positions of the progress bar
				// returned value is clamped between 0 and 1 for lerp
				progress = Mathf.Clamp(ProgressAlongBar(mouseMotion.GlobalPosition), 0.0f, 1.0f);

				//lerp the drag object for progress
				this.GlobalPosition = StartPos.Lerp(EndPos, progress);

				//updates progress bar
				progressBar.Value = progress * progressBar.MaxValue;


				//checks if within range so you cant drag from anywhere on screen
				if (!IsWithinDragRange(mouseMotion.GlobalPosition))
				{
					GD.Print("LeavingDragArea");
					isDragging = false;
				}
			}
		}
	}

	

	//function for calculating the distance of the mousePos along the path between endPos and StartPos
	//Projects mousePos along path to find the distance and progress along the path at that time
	private float ProgressAlongBar(Vector2 MousePos)
	{
		Vector2 PathVec = EndPos - StartPos;
		Vector2 MouseVec = MousePos - StartPos;

		float dist = MouseVec.Project(PathVec).Length() / PathVec.Length();

		return dist;
	}

	//checks if the mouse position is within cursorDragRange
	private bool IsWithinDragRange(Vector2 MousePos)
	{
		float dist = GlobalPosition.DistanceTo(MousePos);

		if(dist <= cursorDragRange)
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
		progressBar.Value -= (BarDepletionRate * delta);

		this.GlobalPosition = StartPos.Lerp(EndPos, (float)progressBar.Value / (float)progressBar.MaxValue);
	}
}
