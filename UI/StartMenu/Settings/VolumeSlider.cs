using Godot;
using System;

public partial class VolumeSlider : HSlider
{
	[Export] public string ModifiedBus = "";
	private bool isDragging = false;
	public override void _Ready()
	{
		DragStarted += () => isDragging = true;
		DragEnded += (valueChanged) => isDragging = false;
		SetBusVolume((float)Value);
	}
	public override void _Process(double delta)
	{
		if (!isDragging) return;
		SetBusVolume((float)Value);
	}
	public void SetBusVolume(float weight)
    {
        int index = AudioServer.GetBusIndex(ModifiedBus);
		if (index == -1)
		{
			GD.PushError($"Bus {ModifiedBus} not found.");
			return;
		}
		float value = Mathf.Lerp(-40f, 0, weight);
		GD.Print(value);
		AudioServer.SetBusVolumeDb(index, value);
    }
}
