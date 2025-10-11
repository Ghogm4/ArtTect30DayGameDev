using Godot;
using System;
[GlobalClass]
public partial class StatModifierComponentResource : Resource
{
    [Export] public float Value = -1f;
    [Export] public StatModifier.OperationType Type = StatModifier.OperationType.BaseAdd;
    [Export] public float ReferencedPercentage = -1f;
    [Export] public string ReferencedStatName = string.Empty;
    [Export] public string TargetStatName = string.Empty;
        public StatModifier CreateModifier(StatComponent statComponent)
    {
        if (statComponent == null)
        {
            GD.PushError("StatComponent is null. Cannot create StatModifier.");
            return null;
        }
        if (!string.IsNullOrEmpty(ReferencedStatName))
            return CreateReferencedModifier(statComponent);
        else
            return CreateNormalModifier(statComponent);
    }
    public StatModifier CreateNormalModifier(StatComponent statComponent)
    {
        if (Value >= 0)
            return new StatModifier(Type, Value);
        else
        {
            GD.PushError("Value must be non-negative.");
            return null;
        }
    }
    public StatModifier CreateReferencedModifier(StatComponent statComponent)
    {
        Stat referencedStat = statComponent.GetStat(ReferencedStatName);
        if (referencedStat == null || ReferencedPercentage < 0)
        {
            GD.PushError($"Stat \"{ReferencedStatName}\" not found or ReferencedPercentage is negative.");
            return null;
        }
        else
        {
            return new StatModifier(Type, referencedStat, ReferencedPercentage);
        }
    }
}
