using Godot;
using System;

public partial class GameData : Node
{
	public static GameData Instance { get; private set; }
	public override void _Ready() => Instance = this;
    public int PlayerHealth = 3;
    public int PlayerMaxHealth = 3;
    public int PlayerShield = 1;
}
