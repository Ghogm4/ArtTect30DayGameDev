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
					if (!IsInstanceValid(ps)) return;
					Boost healthPotion = ResourceLoader.Load<PackedScene>("res://Boosts/General/HealthPotion.tscn").Instantiate<Boost>();
					ps.GetTree()?.CurrentScene?.CallDeferred(Node.MethodName.AddChild, healthPotion);
					healthPotion.Position = pos;
					float spread = Mathf.Pi / 6;
					float direction = (float)GD.RandRange(-Mathf.Pi / 2 - spread, -Mathf.Pi / 2 + spread);
					healthPotion.ApplyCentralImpulse(Vector2.Right.Rotated(direction) * 200f);
                }
			)
		);
    }
}
