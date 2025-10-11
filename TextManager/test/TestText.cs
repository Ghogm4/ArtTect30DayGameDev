using Godot;
using System;

public partial class TestText : Node2D
{
	public override void _Ready()
	{
		TextManager.Instance.LoadLines("res://TextManager/test.json", "scene1");
		TextManager.Instance.LoadTextScene();
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.ShowText);
	}
}
