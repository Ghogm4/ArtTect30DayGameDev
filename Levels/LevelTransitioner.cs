using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class LevelTransitioner : Area2D
{
	public PackedScene TargetLevel = null;
	[Export] public string Entrance = null;
	private bool _isEmitted = false;
	public override void _Ready()
	{
		BodyEntered += (Node2D body) =>
		{
			if (body is Player && !_isEmitted)
			{
				GD.Print(Entrance);
				SignalBus.Instance.EmitSignal(SignalBus.SignalName.EntranceSignal, Entrance);
				_isEmitted = true;
			}
				
		};

		switch (this.Name)
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
				GD.PrintErr($"Unknown LevelTransitioner name: {this.Name}");
				break;
		}
	}
}
