using Godot;
using System;

public partial class InGameMenu : Control
{
	public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ToggleInGameMenu"))
        {
			Visible = !Visible;
        }
    }
}
