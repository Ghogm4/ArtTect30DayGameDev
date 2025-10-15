using Godot;
using System;

public partial class Testenemygen : Node
{
	public Player player = null;
	public MoveHandler SpinningLBW = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as Player;
		SpinningLBW = GetTree().GetRoot().FindChild("SpinningLBW", true, false) as MoveHandler;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			SpinningLBW.StartMove();
		}
	}
}
