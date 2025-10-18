using Godot;
using System;

public partial class LoadSaveButton : ResponsiveButton
{
	private bool _isPressedOnce = false;
	public override void OnPressed()
	{
		if (_isPressedOnce)
			return;
		SceneManager.Instance.ChangeScenePath("res://Levels/Level1.tscn");
		SignalBus.Instance.RegisterSceneChangeStartedAction(() =>
		{
			CanvasLayer playerHealthBar = GetNode<CanvasLayer>("/root/PlayerHealthBar");
			playerHealthBar.Visible = true;
		}, SignalBus.Priority.Low);
		_isPressedOnce = true;
	}
}
