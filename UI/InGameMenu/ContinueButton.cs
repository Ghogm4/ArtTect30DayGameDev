using Godot;
using System;

public partial class ContinueButton : ResponsiveButton
{
    protected override void ReadyBehavior()
    {
        SignalBus.Instance.GameFinished += () => Visible = false;
		SignalBus.Instance.GameStarted += () => Visible = true;
    }

	public override void OnPressed()
	{
		Control InGameMenu = GetNode<Control>("%InGameMenu");
		InGameMenu.Visible = false;
	}
}
