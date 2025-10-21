using Godot;
using System;

public partial class BleedStatModifierComponent : StatModifierComponent
{
    protected override void Modify(StatComponent statComponent, bool reverse = false)
    {
		PlayerStatComponent playerStats = statComponent as PlayerStatComponent;
		playerStats?.AttackActions.Add((StatComponent enemyStats, PlayerStatComponent ps) =>
		{
			float bleedDamage = ps.GetStatValue("Attack");
			float bleedDuration = 5f;
			WeakReference<StatComponent> enemyRef = new(enemyStats);
			Action applyBleed = () =>
			{
				if (enemyRef.TryGetTarget(out StatComponent enemy))
				{
					if (IsInstanceValid(enemy))
					enemy.GetStat("Health").AddFinal(-bleedDamage);
				}
			};
			IntervalTrigger bleedTrigger = new(0, 0.5f, bleedDuration, false, applyBleed);
			IntervalTriggerTicker.Instance.RegisterTrigger(bleedTrigger);
		});
    }
}
