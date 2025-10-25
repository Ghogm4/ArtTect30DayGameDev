using Godot;
using System;

public partial class ContinueButton : ResponsiveButton
{
	public override void OnPressed()
	{
		Control InGameMenu = GetNode<Control>("%InGameMenu");
		InGameMenu.Visible = false;
	}
}
