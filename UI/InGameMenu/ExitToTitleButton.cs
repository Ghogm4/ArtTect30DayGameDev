using Godot;
using System;

public partial class ExitToTitleButton : ResponsiveButton
{
	private bool _isPressedOnce = false;
	public override void OnPressed()
	{
		if (_isPressedOnce)
			return;
		SceneManager.Instance.ChangeScenePath("res://UI/StartMenu/StartMenuExclusive/StartMenu.tscn");
		SignalBus.Instance.RegisterSceneChangeStartedAction(() =>
		{
			Control playerHealthBar = GetNode<Control>("%PlayerHealthBar");
			playerHealthBar.Visible = false;
			_isPressedOnce = false;
		}, SignalBus.Priority.Low);
		_isPressedOnce = true;
		MapManager.Instance.MapPoolIndex = 0;
	}
}
