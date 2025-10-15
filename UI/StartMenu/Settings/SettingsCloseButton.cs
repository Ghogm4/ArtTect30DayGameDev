using Godot;
using System;

public partial class SettingsCloseButton : ResponsiveButton
{
    public override void OnPressed()
    {
		Control root = Owner as Control;
		root.Visible = false;
    }
}
