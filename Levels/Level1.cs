using Godot;
using System;

public partial class Level1 : Node2D
{
	public override void _Ready()
	{
		PlayerHealthBar.Instance.Visible = true;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Restart"))
		{
			RestartScene();
		}
	}
	public void RestartScene()
	{
		SceneManager.Instance.ChangeScene("res://Levels/Level1.tscn");
	}
}
