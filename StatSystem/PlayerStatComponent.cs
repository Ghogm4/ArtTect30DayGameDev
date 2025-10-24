using Godot;
using System;
using System.Collections.Generic;
public partial class PlayerStatComponent : StatComponent
{
    public List<Action<StatComponent, PlayerStatComponent>> AttackActions = new();
    public List<IntervalTrigger> PassiveSkills = new();
    public List<Action<PlayerStatComponent, Vector2>> OnEnemyDeathActions = new();
    private ref bool _initialized => ref GameData.Instance.PlayerStatComponentInitialized;
    public void InitializeOnce()
    {
        if (_initialized) return;
        _initialized = true;
        InitializeDefaultAttackAction();
        InitializeDefaultBoost();
    }
    public override void _Ready()
    {
        base._Ready();
        if (GameData.Instance.StatModifierDict.Count > 0)
            InitializeStatsWithGameData();
        SignalBus.Instance.RegisterSceneChangeStartedAction(() => OnSceneChangeStarted(), SignalBus.Priority.Low);
        SignalBus.Instance.EnemyDied += TriggerOnEnemyDeathActions;
        SignalBus.Instance.PlayerStatResetRequested += ResetStats;
        InitializeOnce();
    }
    private void TriggerOnEnemyDeathActions(Vector2 enemyDeathPos)
    {
        foreach (var action in OnEnemyDeathActions)
            action?.Invoke(this, enemyDeathPos);
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
            float attack = playerStats.GetStatValue("Attack");
            float attackBase = playerStats.GetStatValue("AttackBase");
            float attackMult = playerStats.GetStatValue("AttackMult");
            float attackFinal = playerStats.GetStatValue("AttackFinal");
            float resultDamage = ((attack + attackBase) * attackMult + attackFinal) * resultDamageMultiplier * (resultCritDamage / 100f);
            component.GetStat("Health").AddFinal(-resultDamage);
        };
        AttackActions.Add(initialAttackAction);
    }
    private void InitializeDefaultBoost()
    {
        using Boost flowerOfSerenity = ResourceLoader.Load<PackedScene>("res://Boosts/Special/FlowerOfSerenity.tscn").Instantiate<Boost>();
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerBoostPickedUp, flowerOfSerenity.Info, flowerOfSerenity.NeedDisplay);
    }
    public override void _Process(double delta)
    {
        foreach (var trigger in PassiveSkills)
            trigger.Tick(delta);
    }
    public void SaveStatModifiersToGameData()
    {
        GameData.Instance.StatModifierDict.Clear();
        foreach (var stat in Stats.Values)
            GameData.Instance.StatModifierDict[stat.Name] = stat.CreateModifierResources();
        GameData.Instance.PlayerAttackActions = AttackActions;
        GameData.Instance.PlayerPassiveSkills = PassiveSkills;
        GameData.Instance.PlayerOnEnemyDeathActions = OnEnemyDeathActions;
    }
    public override void ResetStats()
    {
        base.ResetStats();
        GameData.Instance.Reset();
    }
    public void InitializeStatsWithGameData()
    {
        foreach (var pair in GameData.Instance.StatModifierDict)
            foreach (var modifierResource in pair.Value)
                AddModifier(pair.Key, modifierResource.CreateModifier(this));

        AttackActions = GameData.Instance.PlayerAttackActions;
        PassiveSkills = GameData.Instance.PlayerPassiveSkills;
        OnEnemyDeathActions = GameData.Instance.PlayerOnEnemyDeathActions;
    }
    private void OnSceneChangeStarted()
    {
        SaveStatModifiersToGameData();
        SignalBus.Instance.EnemyDied -= TriggerOnEnemyDeathActions;
        QueueFree();
    }
}
