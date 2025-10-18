using Godot;
using System;
using System.Collections;

public partial class BaseLevel : Node2D
{
	[Export] public bool TopExit = false;
	[Export] public bool BottomExit = false;
	[Export] public bool LeftExit = false;
	[Export] public bool RightExit = false;

	[Export] public Node2D TopMarker;
	[Export] public Node2D BottomMarker;
	[Export] public Node2D LeftMarker;
	[Export] public Node2D RightMarker;
	[Export] public Node2D StartMarker;
	[Export] public Node2D EndMarker;

	[Export] public bool IsStartLevel = false;
	
	[Export] public bool IsEndLevel = false;
	[Export] public Player Player = null;

	[Export] public float RarityWeight = 1.0f;

	public Vector2 GetSpawnPosition(string entrance)
	{
		switch (entrance)
		{
			case "Top":
				return TopMarker.GlobalPosition;
			case "Bottom":
				return BottomMarker.GlobalPosition;
			case "Left":
				return LeftMarker.GlobalPosition;
			case "Right":
				return RightMarker.GlobalPosition;
			case "Start":
				return StartMarker.GlobalPosition;
			default:
				GD.PrintErr($"Invalid entrance: {entrance}");
				return Vector2.Zero;
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
