using Godot;
using System;

public partial class TestLevel : Node2D
{

	public override void _Ready()
    {
        PlayerHealthBar.Instance.Visible = true;
    }

}
