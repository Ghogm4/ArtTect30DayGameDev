using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class LevelTransitioner : Area2D
{
	public PackedScene TargetLevel = null;
	public string Entrance = null;
	private bool _isEmitted = false;
	private void ToggleLock(bool enable)
	{
		Monitoring = enable;
		Monitorable = enable;
	}
	public override void _Ready()
	{
		BodyEntered += (Node2D body) =>
		{
			if (body is Player && !_isEmitted)
			{
				GD.Print($"Entrance: {Entrance}");
				SignalBus.Instance.EmitSignal(SignalBus.SignalName.EntranceSignal, Entrance);
				_isEmitted = true;
			}
				
		};

		switch (Name)
		{
			case "LevelTransitionerTop":
				Entrance = "Top";
				break;
			case "LevelTransitionerBottom":
				Entrance = "Bottom";
				break;
			case "LevelTransitionerLeft":
				Entrance = "Left";
				break;
			case "LevelTransitionerRight":
				Entrance = "Right";
				break;
			default:
				GD.PushError($"Unknown LevelTransitioner name: {Name}");
				break;
		}
	}
}
