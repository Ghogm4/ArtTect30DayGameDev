using Godot;
using System;

public partial class Level1 : Node2D
{
	public override void _Ready()
	{
		PlayerHealthBar.Instance.Visible = true;
	}
}
