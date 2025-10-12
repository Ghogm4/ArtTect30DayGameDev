using Godot;
using System;
[GlobalClass]
public partial class StatModifierComponent : Node
{
	[Export] public StatModifierComponentResource[] ModifierResources;
	public void ModifyStatComponent(StatComponent statComponent)
	{
		foreach (var resource in ModifierResources)
		{
			var modifier = resource.CreateModifier(statComponent);
			if (modifier != null)
				statComponent.AddModifier(resource.TargetStatName, modifier);
		}
	}
}
