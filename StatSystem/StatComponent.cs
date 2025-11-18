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
			bool hasExplicitMin = !string.IsNullOrEmpty(minVal);
			bool hasExplicitMax = !string.IsNullOrEmpty(maxVal);
			InitializeStatLimit(stat, minVal, true);
			InitializeStatLimit(stat, maxVal, false);
			stat.DoLimitValidation = hasExplicitMin || hasExplicitMax;
		}
	}
	private void ConnectStatChanged(Stat emitterStat, Stat receiverStat)
	{
		Action<float, float> calculateAction = (a, b) => receiverStat.NeedRefresh = true;
		Callable callable = Callable.From<float, float>(calculateAction);
		if (emitterStat?.IsConnected(Stat.SignalName.StatChanged, callable) ?? true) return;

		emitterStat.Connect(Stat.SignalName.StatChanged, callable);
		GD.Print($"Connected StatChanged signal of {emitterStat.Name} to Calculate method of {receiverStat.Name}");
	}
	private void DisconnectStatChanged(Stat emitterStat, Stat receiverStat)
	{
		Callable callable = Callable.From<float, float>((a, b) => receiverStat.NeedRefresh = true);
		if (!emitterStat?.IsConnected(Stat.SignalName.StatChanged, callable) ?? true) return;

		emitterStat.Disconnect(Stat.SignalName.StatChanged, callable);
		GD.Print($"Disconnected StatChanged signal of {emitterStat.Name} from Calculate method of {receiverStat.Name}");
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
		if (float.TryParse(limitVal, out float constLimit))
		{
			valueProvider = () => constLimit;
		}
		else
		{
			Stat limitStat = GetStat(limitVal);
			valueProvider = () => limitStat?.FinalValue ?? 0f;
			ConnectStatChanged(limitStat, stat);
		}

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
			Stat stat = GetStat(statName);
			stat.AddModifier(modifier);
			ConnectStatChanged(modifier.ReferencedStat, stat);
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
			Stat stat = GetStat(statName);
			stat.RemoveModifier(modifier);
			DisconnectStatChanged(modifier.ReferencedStat, stat);
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
	public void SetValue(string statName, float value) => GetStat(statName)?.SetValue(value);
}
