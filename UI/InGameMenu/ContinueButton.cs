using Godot;
using System;

public partial class ContinueButton : ResponsiveButton
{
	public override void OnPressed()
	{
		CanvasLayer root = Owner as CanvasLayer;
		root.Visible = false;
	}
}
