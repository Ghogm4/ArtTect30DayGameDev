using Godot;
using System;
using System.Collections.Generic;
public partial class PlayerStatComponent : StatComponent
{
    public List<Action<StatComponent, PlayerStatComponent>> AttackActions = new();
    public List<IntervalTrigger> IntervalTriggers = new();
    private ref bool _initialized => ref GameData.Instance.PlayerStatComponentInitialized;
    public void InitializeOnce()
    {
        if (_initialized) return;
        _initialized = true;
        InitializeDefaultAttackAction();
        InitializeDefaultBoost();
    }
    private void InitializeDefaultAttackAction()
    {
        Action<StatComponent, PlayerStatComponent> initialAttackAction = (component, playerStats) =>
        {
            float minDamageMultiplier = playerStats.GetStatValue("MinDamageMultiplier");
            float maxDamageMultiplier = playerStats.GetStatValue("MaxDamageMultiplier");
            float minVal = Mathf.Min(minDamageMultiplier, maxDamageMultiplier);
            float maxVal = Mathf.Max(minDamageMultiplier, maxDamageMultiplier);
            float resultDamageMultiplier = (float)GD.RandRange(minVal, maxVal);

            float critChance = playerStats.GetStatValue("CritChance");
            float critDamage = playerStats.GetStatValue("CritDamage");
            float resultCritDamage = 100f;
            Probability.RunSingle(critChance / 100f, () =>
            {
                resultCritDamage = critDamage;
            });

            float resultDamage = playerStats.GetStatValue("Attack") * resultDamageMultiplier * (resultCritDamage / 100f);
            component.GetStat("Health").AddFinal(-resultDamage);
            GD.Print(playerStats.GetStatValue("Attack"));
        };
        AttackActions.Add(initialAttackAction);
    }
    private void InitializeDefaultBoost()
    {
        using Boost flowerOfSerenity = ResourceLoader.Load<PackedScene>("res://Boosts/Special/FlowerOfSerenity.tscn").Instantiate<Boost>();
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerBoostPickedUp, flowerOfSerenity.Info, flowerOfSerenity.NeedDisplay);
    }
    public override void _Ready()
    {
        if (GameData.Instance.StatModifierDict.Count > 0)
            InitializeStatsWithGameData();
        SignalBus.Instance.RegisterSceneChangeStartedAction(() => OnSceneChangeStarted(), SignalBus.Priority.Low);
        InitializeOnce();
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
