using Godot;
using System;
using Godot.Collections;
[GlobalClass]
public partial class StatComponent : Node
{
	[Export] public Dictionary<string, Stat> Stats = new();
	public Stat GetStat(string statName)
	{
		if (Stats.ContainsKey(statName))
			return Stats[statName];
		GD.PushError($"Stat '{statName}' not found in StatComponent.");
		return null;
	}
    public bool IsStatValueApprox(string statName, float value, float tolerance = 0.01f)
    {
        var stat = GetStat(statName);
        if (stat != null)
            return Mathf.IsEqualApprox(stat.FinalValue, value, tolerance);
        return false;
    }
	public float GetStatValue(string statName)
	{
		return GetStat(statName)?.FinalValue ?? 0f;
	}
	public void AddModifier(string statName, StatModifier modifier)
	{
		if (Stats.ContainsKey(statName))
		{
			Stats[statName].AddModifier(modifier);
			if (!modifier.ReferencedStat?.IsConnected(Stat.SignalName.StatChanged, Callable.From(() => GetStat(statName).Calculate())) ?? false)
				modifier.ReferencedStat.StatChanged += GetStat(statName).Calculate;
		}
		else
			GD.PushError($"Stat '{statName}' not found in StatComponent.");
	}
	public void RemoveModifier(string statName, StatModifier modifier)
	{
		if (Stats.ContainsKey(statName))
		{
			Stats[statName].RemoveModifier(modifier);
			if (modifier.ReferencedStat?.IsConnected(Stat.SignalName.StatChanged, Callable.From(() => GetStat(statName).Calculate())) ?? false)
				modifier.ReferencedStat.StatChanged -= GetStat(statName).Calculate;
		}
		else
			GD.PushError($"Stat '{statName}' not found in StatComponent.");
	}
}
