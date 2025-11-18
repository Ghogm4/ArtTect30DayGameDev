using Godot;
using System;

public partial class PendantOfLifeStatModifierComponent : StatModifierComponent
{
	[Export] public float DamageMultiplier = 0.05f;
	protected override void Modify(StatComponent statComponent, bool reverse = false)
	{
		PlayerStatComponent playerStats = statComponent as PlayerStatComponent;
		if (playerStats == null) return;
		Action<PlayerStatComponent, Vector2> action = (ps, pos) =>
		{
			for (int i = 0; i < (int)ps.GetStatValue("Health") + 1; i++) {
				LifeEnergy lifeEnergy = Projectile.Factory.CreateFriendly<LifeEnergy>("LifeEnergy");
				lifeEnergy.GlobalPosition = pos;
				bool headingLeft = ps.GetStatValue("HeadingLeft") > 0;
				float spread = Mathf.Pi / 17f;
				float radianOffset = (float)GD.RandRange(-spread, spread);
				float radian = (headingLeft ? Mathf.Pi : 0) + radianOffset;
				lifeEnergy.Velocity = lifeEnergy.BaseSpeed * Vector2.Right.Rotated(radian) * (float)GD.RandRange(1f, 1.3f);
				lifeEnergy.Damage = ps.GetAttack() * ps.GetStatValue("ProjectileDamageMultiplier") * DamageMultiplier;
				ps.GetTree().CurrentScene.CallDeferred(MethodName.AddChild, lifeEnergy);
			}
		};
		playerStats.OnAttackActions.Add(action);
	}
}
