using Godot;
using System;

public partial class NurseStatModifierComponent : StatModifierComponent
{
    protected override void Modify(StatComponent statComponent, bool reverse = false)
	{
		PlayerStatComponent playerStats = statComponent as PlayerStatComponent;
		if (playerStats == null) return;

        playerStats.OnEnemyDeathActions.Add(
			(ps, pos) => Probability.RunSingle(
				0.1f,
				() =>
				{
					GD.Print("E");
					if (!IsInstanceValid(ps)) return;
					Boost healthPotion = ResourceLoader.Load<PackedScene>("res://Boosts/General/HealthPotion.tscn").Instantiate<Boost>();
					ps.GetTree()?.CurrentScene?.AddChild(healthPotion);
					healthPotion.Position = pos;
					healthPotion.ApplyCentralImpulse(Vector2.Up * 200f);
                }
			)
		);
    }
}
