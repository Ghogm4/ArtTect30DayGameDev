using Godot;
using System;

public partial class BleedStatModifierComponent : StatModifierComponent
{
    protected override void Modify(StatComponent statComponent, bool reverse = false)
    {
		PlayerStatComponent playerStats = statComponent as PlayerStatComponent;
		playerStats?.OnHittingEnemyAction.Add((StatComponent enemyStats, PlayerStatComponent ps) =>
		{
			float attack = ps.GetStatValue("Attack");
			float attackBase = ps.GetStatValue("AttackBase");
			float attackMult = ps.GetStatValue("AttackMult");
			float attackFinal = ps.GetStatValue("AttackFinal");
			float resultAttack = (attack + attackBase) * attackMult + attackFinal;
			float bleedDamage = Mathf.Ceil(resultAttack * ps.GetStatValue("BleedDamageMultiplier"));
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
