using Godot;
using System;

public partial class StartNewJourneyButton : ResponsiveButton
{
	
	public override void OnPressed()
	{
		GetNode<Control>("%StartNewJourney").Visible = true;
		
	}
}
