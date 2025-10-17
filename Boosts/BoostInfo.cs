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
        Mythical = 2
    }
    public readonly Dictionary<Rarity, Color> RarityColorMap = new()
    {
        [Rarity.Common] = Colors.White,
        [Rarity.Rare] = Colors.Blue,
        [Rarity.Mythical] = Colors.Purple
    };
    [Export] public Rarity BoostRarity = Rarity.Common;
    [Export] public string Name = "";
    [Export] public Texture2D Icon = null;
    [Export] public string Description = "";
    [Export] public int Amount = 0;
}
