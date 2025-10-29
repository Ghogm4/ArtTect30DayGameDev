using Godot;
using System;

public partial class GuardianOfTheForest : EnemyBase
{
    public override void TakeDamage(float damage)
	{
        base.TakeDamage(damage * Mathf.Clamp(1f - Stats.GetStatValue("DamageReduction"), 0f, 1f));
    }
}
