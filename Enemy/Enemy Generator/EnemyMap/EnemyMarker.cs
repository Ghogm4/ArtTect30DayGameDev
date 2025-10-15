using Godot;
using System;
using Godot.Collections;


[GlobalClass]
public partial class EnemyMarker : Node2D
{
	[Export] public Dictionary<string, float> EnemyTypes = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
