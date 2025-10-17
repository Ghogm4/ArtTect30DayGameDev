using Godot;
using System;
using System.Numerics;
[GlobalClass]
public partial class StatModifierResource : Resource
{
	[Export] public float Value = -1f;
	[Export] public StatModifier.OperationType Type = StatModifier.OperationType.BaseAdd;
	[Export] public float ReferencedPercentage = -1f;
	[Export] public string ReferencedStatName = string.Empty;
	[Export] public string TargetStatName = string.Empty;
	public StatModifier CreateModifier(StatComponent statComponent, bool reverse = false)
	{
		if (!string.IsNullOrEmpty(ReferencedStatName))
			return CreateReferencedModifier(statComponent.GetStat(ReferencedStatName), reverse);
		else
			return CreateNormalModifier(reverse);
	}
	public StatModifier CreateNormalModifier(bool reverse)
	{
		StatModifier modifier = new StatModifier(Type, Value);
		return reverse ? modifier.Reverse() : modifier;
	}
	public StatModifier CreateReferencedModifier(Stat referencedStat, bool reverse)
	{
		if (referencedStat == null)
		{
			GD.PushError($"Stat \"{ReferencedStatName}\" not found.");
			return null;
		}
		StatModifier modifier = new StatModifier(Type, referencedStat, ReferencedPercentage);
		return reverse ? modifier.Reverse() : modifier;
	}
}
