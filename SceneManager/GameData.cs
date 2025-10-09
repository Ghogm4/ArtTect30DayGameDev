using Godot;
using System;

public partial class GameData : Node
{
    public static GameData Instance { get; private set; }
    public override void _Ready() => Instance = this;
    public int PlayerHealth = 3;
    public int PlayerMaxHealth = 3;
    public int PlayerShield = 1;
    public int PlayerMaxJumps = 2;
    public int PlayerMaxDashes = 2;
    public float PlayerEvasionChance = 0f;
    public float PlayerCritChance = 0f;
    public float PlayerCritDamageMultiplier = 1f;
    public float PlayerAttackSpeedMultiplier = 1f;
}
