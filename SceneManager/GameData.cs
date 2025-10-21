using Godot;
using System;
using System.Collections.Generic;

public partial class GameData : Node
{
    public static GameData Instance { get; private set; }
    public override void _Ready() => Instance = this;
    public Dictionary<string, List<StatModifierResource>> StatModifierDict = new();
    public List<IntervalTrigger> PlayerPassiveSkills = new();
    public List<Action<StatComponent, PlayerStatComponent>> PlayerAttackActions = new();
    public List<Action<PlayerStatComponent, Vector2>> PlayerOnEnemyDeathActions = new();
    public bool PlayerStatComponentInitialized = false;
}
