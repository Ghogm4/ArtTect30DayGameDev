using Godot;
using System;

public partial class WindSlasherStatModifierComponent : StatModifierComponent
{
    protected override void Modify(StatComponent statComponent, bool reverse = false)
    {
		PlayerStatComponent playerStatComponent = statComponent as PlayerStatComponent;
		if (playerStatComponent == null) return;
		Action<PlayerStatComponent, Vector2> OnPlayerAttack = (playerStat, playerPos) =>
		{
			float attack = playerStat.GetStatValue("Attack");
			float attackBase = playerStat.GetStatValue("AttackBase");
			float attackMult = playerStat.GetStatValue("AttackMult");
			float attackFinal = playerStat.GetStatValue("AttackFinal");
			
			float projectileDamageMultiplier = playerStat.GetStatValue("ProjectileDamageMultiplier");
			float resultAttack = ((attack + attackBase) * attackMult + attackFinal) * 0.8f * projectileDamageMultiplier;

			WindSlash windSlash = Projectile.Factory.CreateFriendly<WindSlash>("WindSlash");
			windSlash.Damage = resultAttack;
			windSlash.Position = playerPos;
			
			float projectileSpeedMultiplier = playerStat.GetStatValue("ProjectileSpeedMultiplier");
			bool headingLeft = playerStat.GetStatValue("HeadingLeft") > 0;
			float spread = Mathf.Pi / 20f;
			float radianOffset = (float)GD.RandRange(-spread, spread);
			int dir = headingLeft ? -1 : 1;
			windSlash.Velocity = dir * Vector2.Right.Rotated(radianOffset) * windSlash.BaseSpeed * projectileSpeedMultiplier;
			windSlash.Rotation = windSlash.Velocity.Angle();

			playerStat.GetTree().CurrentScene.AddChild(windSlash);
		};
		playerStatComponent.OnAttackActions.Add(OnPlayerAttack);
    }
}
