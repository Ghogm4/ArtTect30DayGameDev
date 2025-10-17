using Godot;
using System;
using System.Collections.Generic;

public partial class GameData : Node
{
    public static GameData Instance { get; private set; }
    public override void _Ready() => Instance = this;
    public Dictionary<string, List<StatModifierResource>> StatModifierDict = new();
    public List<IntervalTrigger> PlayerIntervalTriggers = new();
    public List<Action<StatComponent>> PlayerSpecialActions = new();
    public bool PlayerStatComponentInitialized = false;
}
