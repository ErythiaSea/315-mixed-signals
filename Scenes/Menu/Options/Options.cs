using Godot;
using System;

public partial class Options : Control
{
	[Export]
	Slider masterVolumeSlider;

	Button closeButton;

	const string masterBusName = "Master";
	int masterBusIdx;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (masterVolumeSlider == null) 
		{
			GD.PrintErr("A node reference to the master volume slider was not set"); 
		}
		else
		{
			masterVolumeSlider.ValueChanged += UpdateVolume;
		}

		masterBusIdx = AudioServer.GetBusIndex(masterBusName);

		closeButton = GetNode<Button>("Close");
		closeButton.Pressed += CloseOptions;
	}

	private void CloseOptions()
	{
		QueueFree();
	}

	private void UpdateVolume(double value)
	{
		AudioServer.SetBusVolumeDb(masterBusIdx, (float)Mathf.LinearToDb(value));
		GD.Print("new volume is: ", AudioServer.GetBusVolumeDb(masterBusIdx));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
