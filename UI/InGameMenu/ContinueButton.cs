using Godot;
using System;

public partial class ContinueButton : ResponsiveButton
{
    public override void OnPressed()
    {
		Control owner = Owner as Control;
		owner.Visible = false;
    }
}
