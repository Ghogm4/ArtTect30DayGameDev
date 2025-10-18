using Godot;
using System;

[GlobalClass]
public partial class ChangeSceneButton : Button
{
	[Export]
	public string ScenePath = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		if (!string.IsNullOrEmpty(ScenePath))
			SceneManager.Instance.ChangeScenePath(ScenePath);
		else
			GD.PrintErr("未设置目标场景路径！");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
