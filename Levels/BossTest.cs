using Godot;
using System;

public partial class BossTest : BaseLevel
{
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Restart"))
		{
			RestartScene();
		}
	}
	public void RestartScene()
	{
		SceneManager.Instance.ChangeScenePath("res://Levels/BossTest.tscn");
	}
}
