using Godot;
using System;

public partial class ExitButton : ResponsiveButton
{
	public override void OnPressed()
	{
		GetTree().Quit();
	}
}
