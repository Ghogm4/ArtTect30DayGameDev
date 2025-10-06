using Godot;
using System;

public partial class StartJourneyButton : ResponsiveButton
{
	public override void _Ready()
	{
		Pressed += () => SceneManager.Instance.ChangeScene("res://Levels/TestLevel.tscn");
		base._Ready();
	}
}
