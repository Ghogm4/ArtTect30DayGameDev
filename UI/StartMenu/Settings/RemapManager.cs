using Godot;
using System;

public partial class RemapManager : VBoxContainer
{
	private bool _isRemapping = false;
	private RemapButton _currentRemapButton = null;
	public void OnRemapRequested(RemapButton button)
	{
		StopCurrentRemap();
		_isRemapping = true;
		_currentRemapButton = button;
		button.StartRemap();
	}
	public override void _Input(InputEvent @event)
	{
		if (!_isRemapping || _currentRemapButton == null || @event is InputEventMouse) return;
		
		_currentRemapButton.DoRemap(@event);
		_isRemapping = false;
		_currentRemapButton = null;
	}
	public void StopCurrentRemap()
	{
		if (!_isRemapping || _currentRemapButton == null) return;

		_currentRemapButton?.StopRemap();
		_isRemapping = false;
		_currentRemapButton = null;
	}
}
