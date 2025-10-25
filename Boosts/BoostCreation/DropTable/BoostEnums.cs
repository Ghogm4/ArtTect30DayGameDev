using Godot;
using System;

public enum BoostRarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4
}

public enum BoostCategory
{
    None = 0,
    Combat = 1,
    Movement = 2,
    General = 3,
    Survival = 4,
    Hybrid = 5
}

public enum BoostDropMode
{
    Manual = 0,          // Use manually set probabilities
    UniformRarity = 1,   // Drop items of selected rarity with equal probability
    UniformCategory = 2  // Drop items of selected category with equal probability
}
public enum BoostRarityBasedPriceMinFactor
{
    Common = 50,
    Uncommon = 100,
    Rare = 200,
    Epic = 400,
    Legendary = 500
}
public enum BoostRarityBasedPriceMaxFactor
{
    Common = 120,
    Uncommon = 240,
    Rare = 360,
    Epic = 550,
    Legendary = 700
}