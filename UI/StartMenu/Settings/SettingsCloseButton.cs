using Godot;
using System;

public partial class SettingsCloseButton : ResponsiveButton
{
    [Signal] public delegate void ClosedEventHandler();
    public override void OnPressed()
    {
		Control root = Owner as Control;
		root.Visible = false;
        EmitSignal(SignalName.Closed);
    }
}
