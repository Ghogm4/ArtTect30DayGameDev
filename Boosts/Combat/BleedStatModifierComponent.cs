using Godot;
using System;

public partial class BleedStatModifierComponent : StatModifierComponent
{
    protected override void Modify(StatComponent statComponent, bool reverse = false)
    {
		PlayerStatComponent playerStats = statComponent as PlayerStatComponent;
		playerStats?.OnHittingEnemyAction.Add((EnemyBase enemy, PlayerStatComponent ps) =>
		{
			float bleedDamage = Mathf.Ceil(ps.GetAttack() * ps.GetStatValue("BleedDamageMultiplier"));
			float bleedDuration = ps.GetStatValue("BleedDuration");
			float bleedInterval = Mathf.Max(ps.GetStatValue("BleedInterval"), 0.2f);
			Action applyBleed = () =>
			{
				if (IsInstanceValid(enemy))
					enemy.TakeDamage(bleedDamage);
			};
			IntervalTrigger bleedTrigger = new(0, bleedInterval, bleedDuration, false, applyBleed);
			IntervalTriggerTicker.Instance.RegisterTrigger(bleedTrigger);
		});
    }
}
