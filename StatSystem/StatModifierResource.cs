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
	public virtual StatModifier CreateModifier(StatComponent statComponent)
	{
		if (!string.IsNullOrEmpty(ReferencedStatName))
			return CreateReferencedModifier(statComponent.GetStat(ReferencedStatName));
		else
			return CreateNormalModifier();
	}
	public StatModifier CreateNormalModifier() => new StatModifier(Type, Value);
	public StatModifier CreateReferencedModifier(Stat referencedStat)
	{
		if (referencedStat == null)
		{
			GD.PushError($"Stat \"{ReferencedStatName}\" not found.");
			return null;
		}
		return new StatModifier(Type, referencedStat, ReferencedPercentage);
	}
}
