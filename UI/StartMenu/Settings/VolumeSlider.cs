using Godot;
using System;

public partial class VolumeSlider : HSlider
{
	[Export] public string ModifiedBus = "";
	public override void _Ready()
	{
		ValueChanged += SetBusVolume;
		Value = (AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(ModifiedBus)) + 40f) / 40f;
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
		float clamped = Mathf.Clamp((float)weight, 0f, 1f);
		float value = Mathf.Lerp(-40f, 0, clamped);
		AudioServer.SetBusVolumeDb(index, value);
    }
}
