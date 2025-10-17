using Godot;
using System;
using System.Collections.Generic;
public partial class PlayerStatComponent : StatComponent
{
    public List<Action<StatComponent>> AttackActions = new();
    public List<IntervalTrigger> IntervalTriggers = new();
    public override void _Ready()
    {
        if (GameData.Instance.StatModifierDict.Count > 0)
            InitializeStatsWithGameData();
        SignalBus.Instance.RegisterSceneChangeStartedAction(() => OnSceneChangeStarted(), SignalBus.Priority.Low);
    }
    public override void _Process(double delta)
    {
        foreach (var trigger in IntervalTriggers)
            trigger.Tick(delta);
    }
    public void SaveStatModifiersToGameData()
    {
        GameData.Instance.StatModifierDict.Clear();
        foreach (var stat in Stats.Values)
            GameData.Instance.StatModifierDict[stat.Name] = stat.CreateModifierResources();
        GameData.Instance.PlayerSpecialActions = AttackActions;
        GameData.Instance.PlayerIntervalTriggers = IntervalTriggers;
    }
    public void InitializeStatsWithGameData()
    {
        foreach (var pair in GameData.Instance.StatModifierDict)
            foreach (var modifierResource in pair.Value)
                AddModifier(pair.Key, modifierResource.CreateModifier(this));

        AttackActions = GameData.Instance.PlayerSpecialActions;
        IntervalTriggers = GameData.Instance.PlayerIntervalTriggers;
    }
    private void OnSceneChangeStarted()
    {
        SaveStatModifiersToGameData();
        QueueFree();
    }
}
