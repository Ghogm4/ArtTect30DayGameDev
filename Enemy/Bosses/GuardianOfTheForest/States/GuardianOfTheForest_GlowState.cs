using Godot;
using System;

public partial class GuardianOfTheForest_GlowState : State
{
    [ExportGroup("Glow Duration Settings")]
    [Export] public float InitialGlowDuration = 2.0f;
    [Export] public float MaxGlowDuration = 5.0f;
    [ExportGroup("Damage Reduction Settings")]
    [Export] public float DamageReduction = 0.1f;
    [Export] public float MaxDamageReduction = 0.3f;
    [ExportGroup("Orb Launch Settings")]
    [Export] public float OrbLaunchInterval = 1.0f;
    [Export] public float MinOrbLaunchInterval = 0.5f;
    [Export] public int OrbCount = 12;
    [Export] public float OrbSpeed = 350f;
    [Export] public float MaxOrbSpeed = 500f;
    [ExportGroup("Glow Speed Settings")]
    [Export] public float MaxGlowSpeed = 3.0f;
    [ExportGroup("Move Settings")]
    [Export] public float MoveSpeedMultiplier = 0.5f;
    [Export] public float MaxMoveSpeedMultiplier = 1.0f;
    [Export] public float AccelerationMagnitude = 100f;
    [Export] public float MaxAccelerationMagnitude = 400f;
    [Export] public PackedScene BlueOrbScene = null;
    private Vector2 PlayerPos => (GetTree().GetFirstNodeInGroup("Player") as Player).GlobalPosition;
    private float Health => Stats.GetStatValue("Health");
    private float MaxHealth => Stats.GetStatValue("MaxHealth");
    private float Ratio => Mathf.Clamp(Health / MaxHealth, 0, 1);
    private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy = null;
    private float _damageReductionApplied = 0f;
    private float _currentOrbLaunchInterval = 0f;
    private float _glowDuration = 0f;
    private float _previousDamageReduction = 0f;
    private float _elapsedTime = 0f;
    private float _orbSpeed = 0f;
    private float _moveSpeed = 60f;
    private float _moveSpeedMultiplier = 0f;
    private float _accelerationMagnitude = 200f;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
    }
    protected override void Enter()
    {
        _previousDamageReduction = Stats.GetStatValue("DamageReduction");
        _glowDuration = Mathf.Lerp(MaxGlowDuration, InitialGlowDuration, Ratio);
        _sprite.Play("Glow");
        _enemy.Velocity = Vector2.Zero;
        GetTree().CreateTimer(_glowDuration).Timeout += () =>
        {
            if (IsInstanceValid(this) && !_enemy.IsDead)
                AskTransit("Decision");
        };
    }
    private void ModifyData()
    {
        _damageReductionApplied = Mathf.Lerp(MaxDamageReduction, DamageReduction, Ratio);
        Stats.SetValue("DamageReduction", _previousDamageReduction + _damageReductionApplied);
        _currentOrbLaunchInterval = Mathf.Lerp(MinOrbLaunchInterval, OrbLaunchInterval, Ratio);
        _orbSpeed = Mathf.Lerp(MaxOrbSpeed, OrbSpeed, Ratio);
        _moveSpeedMultiplier = Mathf.Lerp(MaxMoveSpeedMultiplier, MoveSpeedMultiplier, Ratio);
        _accelerationMagnitude = Mathf.Lerp(MaxAccelerationMagnitude, AccelerationMagnitude, Ratio);
    }
    protected override void FrameUpdate(double delta)
    {
        ModifyData();
        _sprite.SpeedScale = Mathf.Lerp(MaxGlowSpeed, 1.0f, Ratio);
        _elapsedTime += (float)delta;
        if (_elapsedTime >= _currentOrbLaunchInterval)
        {
            LaunchOrb();
            _elapsedTime = 0f;
        }
        Vector2 direction = (PlayerPos - _enemy.GlobalPosition).Normalized();
        Vector2 acceleration = direction * _accelerationMagnitude;
        _enemy.Velocity = (_enemy.Velocity + acceleration * (float)delta).Normalized() * _moveSpeed * _moveSpeedMultiplier;
        _enemy.MoveAndSlide();
    }
    protected override void Exit()
    {
        Stats.SetValue("DamageReduction", _previousDamageReduction);
    }
    private void LaunchOrb()
    {
        float currentRadian = (float)GD.RandRange(0, Mathf.Tau);
        float radianIncrement = Mathf.Tau / OrbCount;
        for (int i = 0; i < OrbCount; i++)
        {
            SpawnOrb(_orbSpeed, currentRadian, true);
            currentRadian += radianIncrement;
        }
    }
    private void SpawnOrb(float speed, float radian, bool canPierceWorld)
    {
        var orb = BlueOrbScene.Instantiate<BlueOrb>();
        orb.GlobalPosition = _enemy.GlobalPosition;
        orb.Velocity = Vector2.Right.Rotated(radian) * speed;
        orb.Rotation = orb.Velocity.Angle();
        orb.PierceWorld = canPierceWorld;
        GetTree().CurrentScene.CallDeferred(MethodName.AddChild, orb);
    }
}
