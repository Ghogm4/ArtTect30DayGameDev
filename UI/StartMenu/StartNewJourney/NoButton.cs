using Godot;
using System;

public partial class NoButton : Button
{
	public override void _Pressed()
	{
		Control root = Owner as Control;
		root.Visible = false;
	}
}
