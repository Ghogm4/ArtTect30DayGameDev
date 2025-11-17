using Godot;
using System;

public partial class TutorialButton : ResponsiveButton
{
	public override void OnPressed()
	{
		GetNode<Control>("%Tutorial").Visible = true;
	}
}
