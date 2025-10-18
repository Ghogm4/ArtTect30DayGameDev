using Godot;
using System;

public partial class PrintStatModifierComponent : StatModifierComponent
{
	protected override void Modify(StatComponent statComponent, bool reverse = false)
	{
		IntervalTrigger trigger = new(0, 0.1f, false, () =>
		{
			GD.Print("Triggered");
		});
		PlayerStatComponent component = statComponent as PlayerStatComponent;
		component?.IntervalTriggers.Add(trigger);
		component?.AttackActions.Add((StatComponent s) =>
		{
			GD.PrintErr("Special Action Executed");
		});
	}
}
