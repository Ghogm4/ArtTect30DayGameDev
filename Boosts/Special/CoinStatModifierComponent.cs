using Godot;
using System;

public partial class CoinStatModifierComponent : StatModifierComponent
{
	private bool _modified = false;

    protected override void Modify(StatComponent statComponent, bool reverse = false)
	{
		if (_modified) return;
		Boost boost = Owner as Boost;
		if (boost == null)
			return;
		int coinCount = boost.Info.Amount;
		statComponent.AddFinal("Coin", coinCount * statComponent.GetStatValue("CoinMultiplier"));
		_modified = true;
    }
}
