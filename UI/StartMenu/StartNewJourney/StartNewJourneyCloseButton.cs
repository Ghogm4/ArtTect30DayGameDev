using Godot;
using System;

public partial class StartNewJourneyCloseButton : ResponsiveButton
{
    protected override void ReadyBehavior()
    {
        PivotOffset = Size / 2;
    }

	public override void OnPressed()
	{
		Control root = Owner as Control;
		root.Visible = false;
	}
}
