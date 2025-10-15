using Godot;
using System;

public partial class StartNewJourneyCloseButton : ResponsiveButton
{
    public override void OnPressed()
    {
        Control root = Owner as Control;
		root.Visible = false;
    }
}
