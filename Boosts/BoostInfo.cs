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
    [Export(PropertyHint.MultilineText)] public string Description = "";
    [Export] public int Amount = 0;
    [Export] public bool IsOneTimeOnly = false; // 是否为一次性增益（获得后不再出现）
}
