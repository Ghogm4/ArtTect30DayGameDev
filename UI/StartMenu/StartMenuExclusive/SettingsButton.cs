using Godot;
using System;

public partial class SettingsButton : ResponsiveButton
{
    [Export] public bool DisconnectOnTreeExiting = false;
    protected override void ReadyBehavior()
    {
        SignalBus.Instance.GameFinished += DisableVisible;
		SignalBus.Instance.GameStarted += EnableVisible;
    }

    public override void OnPressed()
    {
        GetNode<Control>("%Settings").Visible = true;
    }
    public override void _ExitTree()
    {
        if (DisconnectOnTreeExiting)
        {
            SignalBus.Instance.GameFinished -= DisableVisible;
            SignalBus.Instance.GameStarted -= EnableVisible;
        }
    }

    private void DisableVisible() => Visible = false;
    private void EnableVisible() => Visible = true;
}
