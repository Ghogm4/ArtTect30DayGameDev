using Godot;
using System;
using System.Threading.Tasks;

public partial class YesButton : Button
{
	private bool _isPressedOnce = false;
	public override void _Pressed()
	{
		if (_isPressedOnce)
			return;

		_isPressedOnce = true;
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerStatResetRequested);
		MapManager.Instance.InitMaps();
		MapManager.Instance.StartLevel();
	}
}
