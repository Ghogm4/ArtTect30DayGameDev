using Godot;
using System;

public partial class SettingsButton : ResponsiveButton
{
	public override void OnPressed()
	{
		GetNode<Control>("%Settings").Visible = true;
	}
}
