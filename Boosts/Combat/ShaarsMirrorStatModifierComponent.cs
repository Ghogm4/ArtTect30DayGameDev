using Godot;
using System;

public partial class ShaarsMirrorStatModifierComponent : StatModifierComponent
{
    protected override void Modify(StatComponent statComponent, bool reverse = false)
    {
        PlayerStatComponent playerStats = statComponent as PlayerStatComponent;
		if (playerStats == null) return;
		playerStats.OnAttackActions.Add((ps, pos) =>
        {
			bool trigger = GD.Randf() < 0.25f;
			if (!trigger) return;
            foreach (var action in ps.OnAttackActions)
				action?.Invoke(ps, pos);
        });
    }
}
