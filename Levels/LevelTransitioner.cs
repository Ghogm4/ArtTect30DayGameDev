using Godot;
using System;

public partial class LevelTransitioner : Area2D
{
	[Export] public string TargetLevelPath = "";
	public override void _Ready()
	{
		BodyEntered += (Node2D body) =>
		{
			if (body is Player)
				SceneManager.Instance.ChangeScene(TargetLevelPath);
		};
	}
}
