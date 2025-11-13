using Godot;
using System;
using System.Collections.Generic;
public partial class PlayerStatComponent : StatComponent
{
    public List<Action<EnemyBase, PlayerStatComponent>> OnHittingEnemyAction = new();
    public List<IntervalTrigger> PassiveSkills = new();
    public List<Action<EnemyBase, PlayerStatComponent>> OnEnemyDeathActions = new();
    // Vector2 is the position of the player when attacking
    public List<Action<PlayerStatComponent, Vector2>> OnAttackActions = new();
    public List<Action<PlayerStatComponent, Vector2>> OnJumpActions = new();
    public List<Action<PlayerStatComponent, Vector2>> OnDashActions = new();
    public List<Func<float, PlayerStatComponent, float>> DamageCalculators = new();
    private ref bool _initialized => ref GameData.Instance.PlayerStatComponentInitialized;
    public void InitializeOnce()
    {
        if (_initialized) return;
        _initialized = true;
        InitializeDefaultAttackAction();
        InitializeDefaultBoost();
    }
    public float GetAttack()
    {
        float attack = GetStatValue("Attack");
        float attackBase = GetStatValue("AttackBase");
        float attackMult = GetStatValue("AttackMult");
        float attackFinal = GetStatValue("AttackFinal");
        return (attack + attackBase) * attackMult + attackFinal;
    }
    public float GetCritDamageMultiplier()
    {
        float critChance = GetStatValue("CritChance");
        float critDamage = GetStatValue("CritDamage");
        float resultCritDamage = 100f;
        Probability.RunSingle(critChance / 100f, () =>
        {
            resultCritDamage = critDamage;
        });
        return resultCritDamage / 100f;
    }
    public float GetRandomDamageMultiplier()
    {
        float minDamageMultiplier = GetStatValue("MinDamageMultiplier");
        float maxDamageMultiplier = GetStatValue("MaxDamageMultiplier");
        float minVal = Mathf.Min(minDamageMultiplier, maxDamageMultiplier);
        float maxVal = Mathf.Max(minDamageMultiplier, maxDamageMultiplier);
        float resultDamageMultiplier = (float)GD.RandRange(minVal, maxVal);
        return resultDamageMultiplier;
    }
    public float CalculateDamage()
    {
        float damageMultiplier = GetRandomDamageMultiplier();

        float attack = GetAttack();

        float critDamageMultiplier = GetCritDamageMultiplier();
        float damage = attack * damageMultiplier * critDamageMultiplier;
        foreach (var calculator in DamageCalculators)
            damage = calculator(damage, this);
        
        return damage;
    }
    public override void _Ready()
    {
        base._Ready();
        if (GameData.Instance.StatModifierDict.Count > 0)
            InitializeStatsWithGameData();
        SignalBus.Instance.RegisterSceneChangeStartedAction(() => OnSceneChangeStarted(), SignalBus.Priority.Low);
        SignalBus.Instance.EnemyDied += TriggerOnEnemyDeathActions;
        SignalBus.Instance.PlayerStatResetRequested += ResetStats;
        SignalBus.Instance.PlayerPurchased += (int price) => AddFinal("Coin", -price);
        Player player = Owner as Player;
        player.PlayerJumped += TriggerOnPlayerJumpedActions;
        player.PlayerDashed += TriggerOnPlayerDashedActions;
        AddFinal("AvailableDashes", GetStatValue("MaxDash") - GetStatValue("AvailableDashes"));
        InitializeOnce();
    }
    private void TriggerOnEnemyDeathActions(EnemyBase deadEnemy)
    {
        foreach (var action in OnEnemyDeathActions)
            action?.Invoke(deadEnemy, this);
    }
    private void TriggerOnPlayerJumpedActions()
    {
        foreach (var action in OnJumpActions)
            action?.Invoke(this, (Owner as Player).GlobalPosition);
    }
    private void TriggerOnPlayerDashedActions()
    {
        foreach (var action in OnDashActions)
            action?.Invoke(this, (Owner as Player).GlobalPosition);
    }
    private void InitializeDefaultAttackAction()
    {
        Action<EnemyBase, PlayerStatComponent> initialAttackAction = (enemy, playerStats) =>
        {
            enemy.TakeDamage(playerStats.CalculateDamage());
        };
        OnHittingEnemyAction.Add(initialAttackAction);
    }
    private void InitializeDefaultBoost()
    {
        using Boost flowerOfSerenity = ResourceLoader.Load<PackedScene>("res://Boosts/Special/FlowerOfSerenity.tscn").Instantiate<Boost>();
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerBoostPickedUp,
            flowerOfSerenity.Info, flowerOfSerenity.DisplayWhenObtained, flowerOfSerenity.DisplayOnCurrentBoosts);
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
        SaveActionsToGameData();
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

        LoadActionsFromGameData();
    }
    private void OnSceneChangeStarted()
    {
        SaveStatModifiersToGameData();
        SignalBus.Instance.EnemyDied -= TriggerOnEnemyDeathActions;
        QueueFree();
    }
    private void SaveActionsToGameData()
    {
        GameData.Instance.PlayerOnHittingEnemyActions = OnHittingEnemyAction;
        GameData.Instance.PlayerPassiveSkills = PassiveSkills;
        GameData.Instance.PlayerOnEnemyDeathActions = OnEnemyDeathActions;
        GameData.Instance.PlayerOnAttackActions = OnAttackActions;
        GameData.Instance.PlayerOnJumpActions = OnJumpActions;
        GameData.Instance.PlayerOnDashActions = OnDashActions;
        GameData.Instance.PlayerDamageCalculators = DamageCalculators;
    }
    private void LoadActionsFromGameData()
    {
        OnHittingEnemyAction = GameData.Instance.PlayerOnHittingEnemyActions;
        PassiveSkills = GameData.Instance.PlayerPassiveSkills;
        OnEnemyDeathActions = GameData.Instance.PlayerOnEnemyDeathActions;
        OnAttackActions = GameData.Instance.PlayerOnAttackActions;
        OnJumpActions = GameData.Instance.PlayerOnJumpActions;
        OnDashActions = GameData.Instance.PlayerOnDashActions;
        DamageCalculators = GameData.Instance.PlayerDamageCalculators;
    }
}
