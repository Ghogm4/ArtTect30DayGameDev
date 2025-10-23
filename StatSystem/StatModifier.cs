using Godot;
using System;

public partial class StatModifier : RefCounted
{
	public enum OperationType { BaseAdd, Mult, FinalAdd }
	public OperationType Type { get; private set; } = OperationType.BaseAdd;
	public Stat ReferencedStat { get; private set; } = null;
	private float _referencedPercentage = 100f;
	private float _value = 0;
	public float Value
	{
		get
		{
			return ReferencedStat != null ? ReferencedStat.FinalValue * _referencedPercentage / 100f : _value;
		}
		private set => _value = value;
	}
	public StatModifier(OperationType type, float value)
	{
		Type = type;
		Value = value;
	}
	public StatModifier(OperationType type, Stat referencedStat, float referencedPercentage = 100f)
	{
		Type = type;
		ReferencedStat = referencedStat;
		_referencedPercentage = referencedPercentage;
	}
	public float Operate(float value)
	{
		return Type switch
		{
			OperationType.BaseAdd or OperationType.FinalAdd => value + Value,
			OperationType.Mult => value * Value,
			_ => Value
		};
	}
	public StatModifier Reverse()
	{
		if (Type is OperationType.BaseAdd or OperationType.FinalAdd)
			Value = -Value;
		else if (Type is OperationType.Mult)
			Value = Mathf.IsZeroApprox(Value) ? 0 : 1f / Value;
		return this;
	}
	public StatModifierResource CreateResource(string targetStatName)
    {
		StatModifierResource resource = new();
		resource.Type = Type;
		resource.Value = _value;
		resource.TargetStatName = targetStatName;
		if (ReferencedStat == null)
			return resource;
		resource.ReferencedStatName = ReferencedStat.Name;
		resource.ReferencedPercentage = _referencedPercentage;
		return resource;
    }
	public class Factory
	{
		public static StatModifier BaseAdd(float value) => new(OperationType.BaseAdd, value);
		public static StatModifier Mult(float value) => new(OperationType.Mult, value);
		public static StatModifier FinalAdd(float value) => new(OperationType.FinalAdd, value);
	}
}
