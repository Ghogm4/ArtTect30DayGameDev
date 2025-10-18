using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class BoostInfo : Resource
{
    public static Dictionary<BoostRarity, Color> RarityColorMap = new()
    {
        [BoostRarity.Common] = Colors.White,
        [BoostRarity.Uncommon] = Colors.Green,
        [BoostRarity.Rare] = Colors.Blue,
        [BoostRarity.Epic] = Colors.Purple,
        [BoostRarity.Legendary] = Colors.Orange
    };
    
    [Export] public BoostRarity Rarity = BoostRarity.Common;
    [Export] public BoostCategory Category = BoostCategory.None;
    [Export] public string Name = "";
    [Export] public Texture2D Icon = null;
    [Export] public string Description = "";
    [Export] public int Amount = 0;
}
