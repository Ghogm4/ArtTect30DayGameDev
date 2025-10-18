using Godot;
using System;

public partial class LevelTransitioner : Area2D
{
	public PackedScene TargetLevel = null;
	[Export] public string Entrance = null;
	
	public override void _Ready()
	{
		BodyEntered += (Node2D body) =>
		{
			if (body is Player)
			{
				EmitSignal(SignalBus.SignalName.EntranceSignal, Entrance);
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
