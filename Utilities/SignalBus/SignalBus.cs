using Godot;
using System;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }
    public override void _Ready() => Instance = this;
    [Signal] public delegate void PlayerHitEventHandler();
}
