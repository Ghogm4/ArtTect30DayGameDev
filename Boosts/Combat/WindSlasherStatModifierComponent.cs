using Godot;
using System;

public partial class WindSlasherStatModifierComponent : StatModifierComponent
{
	[Export] public float AttackMultiplier = 0.7f;
    protected override void Modify(StatComponent statComponent, bool reverse = false)
    {
		PlayerStatComponent playerStatComponent = statComponent as PlayerStatComponent;
		if (playerStatComponent == null) return;
		Action<PlayerStatComponent, Vector2> OnPlayerAttack = (playerStat, playerPos) =>
		{
			float projectileDamageMultiplier = playerStat.GetStatValue("ProjectileDamageMultiplier");
			float resultAttack = playerStat.GetAttack() * AttackMultiplier * projectileDamageMultiplier;

			WindSlash windSlash = Projectile.Factory.CreateFriendly<WindSlash>("WindSlash");
			windSlash.Damage = resultAttack;
			windSlash.Position = playerPos;
			
			float projectileSpeedMultiplier = playerStat.GetStatValue("ProjectileSpeedMultiplier");
			bool headingLeft = playerStat.GetStatValue("HeadingLeft") > 0;
			float spread = Mathf.Pi / 20f;
			float radianOffset = (float)GD.RandRange(-spread, spread);
			float speedMultiplierOffset = (float)GD.RandRange(0.9f, 1f);
			int dir = headingLeft ? -1 : 1;
			windSlash.Velocity = dir * Vector2.Right.Rotated(radianOffset) * windSlash.BaseSpeed * projectileSpeedMultiplier * speedMultiplierOffset;
			windSlash.Rotation = windSlash.Velocity.Angle();

			playerStat.GetTree().CurrentScene.AddChild(windSlash);
		};
		playerStatComponent.OnAttackActions.Add(OnPlayerAttack);
    }
}
