using Godot;
using System;

public partial class GuardianOfTheForest : EnemyBase
{
    public override void TakeDamage(float damage)
	{
        base.TakeDamage(damage * (1f - Stats.GetStatValue("DamageReduction")));
    }
}
