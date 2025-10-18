using Godot;
using System;

public partial class LevelTransitioner : Area2D
{
	public PackedScene TargetLevel = null;
	public override void _Ready()
	{
		BodyEntered += (Node2D body) =>
		{
			if (body is Player)
				SceneManager.Instance.ChangeScene(TargetLevel);
		};
	}
}
