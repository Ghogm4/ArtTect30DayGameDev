using Godot;
using System;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }
    public override void _Ready() => Instance = this;
    [Signal] public delegate void PlayerHitEventHandler();
    [Signal] public delegate void PlayerHealthStatusUpdatedEventHandler(int health, int maxHealth, int shield);
    [Signal] public delegate void PlayerDiedEventHandler();
    [Signal] public delegate void ShowTextEventHandler();
    [Signal] public delegate void WaitAdvanceEventHandler();
}
