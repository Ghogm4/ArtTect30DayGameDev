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
	public class Factory
	{
		public static StatModifier BaseAdd(float value) => new(OperationType.BaseAdd, value);
		public static StatModifier Mult(float value) => new(OperationType.Mult, value);
		public static StatModifier FinalAdd(float value) => new(OperationType.FinalAdd, value);
	}
}
