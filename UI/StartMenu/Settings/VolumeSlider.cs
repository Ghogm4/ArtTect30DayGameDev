using Godot;
using System;

public partial class VolumeSlider : HSlider
{
	[Export] public string ModifiedBus = "";
	public override void _Ready()
	{
		ValueChanged += SetBusVolume;
		SetBusVolume(Value);
	}
	public void SetBusVolume(double weight)
    {
        int index = AudioServer.GetBusIndex(ModifiedBus);
		if (index == -1)
		{
			GD.PushError($"Bus {ModifiedBus} not found.");
			return;
		}
		float value = Mathf.Lerp(-40f, 0, (float)weight);
		AudioServer.SetBusVolumeDb(index, value);
    }
}
