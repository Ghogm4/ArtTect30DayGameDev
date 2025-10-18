using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class BoostInfo : Resource
{
    public enum Rarity
    {
        Common = 0,
        Rare = 1,
        Epic = 2,
        Mythical = 3
    }
    public static Dictionary<Rarity, Color> RarityColorMap = new()
    {
        [Rarity.Common] = Colors.White,
        [Rarity.Rare] = Colors.Blue,
        [Rarity.Epic] = Colors.Purple,
        [Rarity.Mythical] = Colors.Crimson
    };
    [Export] public Rarity BoostRarity = Rarity.Common;
    [Export] public string Name = "";
    [Export] public Texture2D Icon = null;
    [Export] public string Description = "";
    [Export] public int Amount = 0;
}
