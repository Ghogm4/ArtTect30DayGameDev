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
				GD.Print("Changed");
				SignalBus.Instance.EmitSignal(SignalBus.SignalName.EntranceSignal, Entrance);
				_isEmitted = true;
			}
				
		};

		switch (this.Name)
		{
			case "LevelTransitionerTop":
				Entrance = "Bottom";
				break;
			case "LevelTransitionerBottom":
				Entrance = "Top";
				break;
			case "LevelTransitionerLeft":
				Entrance = "Right";
				break;
			case "LevelTransitionerRight":
				Entrance = "Left";
				break;
			default:
				GD.PrintErr($"Unknown LevelTransitioner name: {this.Name}");
				break;
		}
	}
}
