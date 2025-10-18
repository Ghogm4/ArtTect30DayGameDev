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
			CanvasLayer playerHealthBar = GetNode<CanvasLayer>("/root/PlayerHealthBar");
			playerHealthBar.Visible = false;
		}, SignalBus.Priority.Low);
		_isPressedOnce = true;
	}
}
