using Godot;
using System;

public partial class YesButton : Button
{
	private bool _isPressedOnce = false;
	public override void _Pressed()
	{
		if (_isPressedOnce)
			return;

		_isPressedOnce = true;
		MapManager.Instance.InitMaps();
		MapManager.Instance.StartLevel();
	}
}
