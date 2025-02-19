using Godot;
using System;

public partial class DragProgressBar : ProgressBar
{
	ShaderMaterial shaderMat;
	DraggableRect childDragger;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		childDragger = this.GetChild<DraggableRect>(0);
		shaderMat = Material.Duplicate() as ShaderMaterial;
		Material = shaderMat;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(childDragger.progress > 0)
		{
			float progressVal = (float)this.Value / (float)this.MaxValue;
			shaderMat.SetShaderParameter("progress", progressVal);
		}
	}
}
