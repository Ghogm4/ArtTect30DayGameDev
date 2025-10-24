using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
[GlobalClass]
public partial class StatComponent : Node
{
	[Export] public Godot.Collections.Dictionary<string, Stat> Stats = new();
	public override void _Ready()
	{
		foreach (var stat in Stats.Values)
		{
			string minVal = stat.MinValue;
			string maxVal = stat.MaxValue;
			InitializeStatLimit(stat, minVal, true);
			InitializeStatLimit(stat, maxVal, false);
		}
	}
	private void InitializeStatLimit(Stat stat, string limitVal, bool processMin)
	{
		if (string.IsNullOrEmpty(limitVal))
        {
            if (processMin)
				stat.MinLimit.ValueProvider = () => int.MinValue;
			else
				stat.MaxLimit.ValueProvider = () => int.MaxValue;
			return;
        }
		Func<float> valueProvider;
		if (int.TryParse(limitVal, out int constLimit))
			valueProvider = () => constLimit;
		else
			valueProvider = () => GetStat(limitVal)?.FinalValue ?? 0f;

		if (processMin)
			stat.MinLimit.ValueProvider = valueProvider;
		else
			stat.MaxLimit.ValueProvider = valueProvider;
	}
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
	public void AddBuff(string statName, StatModifier modifier, float duration = 1f, bool clearOnSceneChange = true)
	{
		AddModifier(statName, modifier);
		if (this is not PlayerStatComponent)
		{
			Scheduler.Instance.ScheduleAction(duration, () => RemoveModifier(statName, modifier), 10, true);
			return;
		}

		if (clearOnSceneChange)
			Scheduler.Instance.ScheduleAction(duration, () => RemoveModifier(statName, modifier), 10, true);
		else
			Scheduler.Instance.ScheduleAction(duration, () =>
			{
				PlayerStatComponent playerStats = Scheduler.Instance.GetTree().GetFirstNodeInGroup("Player").GetNode<PlayerStatComponent>("StatComponent");
				playerStats?.AddModifier(statName, modifier.CreateResource(statName).CreateModifier(playerStats, true));
			}, 10, false);
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
	public virtual void ResetStats()
    {
        foreach (var stat in Stats.Values)
			stat.Reset();
    }
	public void AddBase(string statName, float value) => GetStat(statName)?.AddBase(value);
	public void Mult(string statName, float multiplier) => GetStat(statName)?.Mult(multiplier);
	public void AddFinal(string statName, float value) => GetStat(statName)?.AddFinal(value);
}
