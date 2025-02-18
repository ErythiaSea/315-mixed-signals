using Godot;
using System;

public partial class DraggableRect : TextureRect
{
	Vector2 StartPos;
	Vector2 EndPos;
	Vector2 offset = new Vector2();
	float progress;
	bool isDragging;
	bool isCompleted = false;
	ProgressBar progressBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		progressBar = this.GetParent<ProgressBar>();

        StartPos = progressBar.GlobalPosition;
		EndPos = StartPos + new Vector2(progressBar.Size.X, 0).Rotated(progressBar.Rotation);
		
	
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(progress);
		if (progress >= 0.99f)
		{
			isCompleted = true;
		}

	}

	public void MouseInput(InputEvent @event)
	{
		//Check how far away the mouse is from the draggable object, once over a certain thres, stop dragging
		if (!isCompleted)
		{
            if (@event is InputEventMouseButton mouseButton)
            {
                isDragging = mouseButton.ButtonIndex == MouseButton.Left;

                if (mouseButton.IsReleased())
                {
                    isDragging = false;
                }
            }

            if (@event is InputEventMouseMotion mouseMotion && isDragging)
            {
                GD.Print("moving");

                //progress gotta be the position betwwen start and end clamped 

                progress = Mathf.Clamp(ProgressAlongBar(mouseMotion.GlobalPosition), 0.0f, 1.0f);

                this.GlobalPosition = StartPos.Lerp(EndPos, progress);

				progressBar.Value = progress * progressBar.MaxValue;
            }
        }
    }

	

	private float ProgressAlongBar(Vector2 MousePos)
	{
		Vector2 PathVec = EndPos - StartPos;
		Vector2 MouseVec = MousePos - StartPos;

		float dist = MouseVec.Project(PathVec).Length() / PathVec.Length();
		return dist;
	}
}
