using Godot;
using System;

public partial class BleedStatModifierComponent : StatModifierComponent
{
    protected override void Modify(StatComponent statComponent, bool reverse = false)
    {
		PlayerStatComponent playerStats = statComponent as PlayerStatComponent;
		playerStats?.AttackActions.Add((StatComponent enemyStats, PlayerStatComponent ps) =>
		{
			float bleedDamage = Mathf.Ceil(ps.GetStatValue("Attack") * ps.GetStatValue("BleedDamageMultiplier"));
			float bleedDuration = ps.GetStatValue("BleedDuration");
			float bleedInterval = ps.GetStatValue("BleedInterval");
			Action applyBleed = () =>
			{
				if (IsInstanceValid(enemyStats))
					enemyStats.GetStat("Health").AddFinal(-bleedDamage);
			};
			IntervalTrigger bleedTrigger = new(0, bleedInterval, bleedDuration, false, applyBleed);
			IntervalTriggerTicker.Instance.RegisterTrigger(bleedTrigger);
		});
    }
}
