using Godot;
using System;
[GlobalClass]
public partial class StatModifierComponent : Node
{
	[Export] public StatModifierResource[] ModifierResources;
	protected virtual void Modify(StatComponent statComponent, bool reverse = false) { }
	public void ModifyStatComponent(StatComponent statComponent, bool reverse = false)
	{
		foreach (var resource in ModifierResources)
		{
			var modifier = reverse ? resource.CreateModifier(statComponent).Reverse() : resource.CreateModifier(statComponent);
			if (modifier != null)
				statComponent.AddModifier(resource.TargetStatName, modifier);
		}
		Modify(statComponent, reverse);
	}
}
