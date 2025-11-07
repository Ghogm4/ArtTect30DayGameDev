using Godot;
using System;

public partial class LargeBat_SummonState : State
{
    [Export] public float LockRadius = 0f;
    [Export] public float LockWait = 1f;
    [Export] public float DashDuration = 1f;
    [Export] public float DashSpeedMultiplier = 1.5f;
    public bool LockedOnPlayer
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            if (field)
            {
                _dashDirection = (PlayerPos - EnemyPos).Normalized();
                GetTree().CreateTimer(DashDuration).Timeout += () =>
                {
                    if (!IsInstanceValid(_enemy)) return;
                    CallDeferred(MethodName.ToggleAttackArea, false);
                    AskTransit("Decision");
                };
            }
        }
    } = false;
    private Vector2 PlayerPos
    {
        get
        {
            Player player = Storage.GetNode<Player>("Player");
            return player?.GlobalPosition ?? Vector2.Zero;
        }
    }
    private Vector2 EnemyPos => _enemy.GlobalPosition;
    private Vector2 RandomPosAroundPlayer
    {
        get
        {
            float randomRadian = (float)GD.RandRange(0, Mathf.Tau);
            return PlayerPos + Vector2.Right.Rotated(randomRadian) * LockRadius;
        }
    }
    private bool _startAttack = false;
    private Vector2 _lockPos = Vector2.Zero;
    private Vector2 _dashDirection = Vector2.Zero;
    private float _timeElapsed = 0f;
    private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy;
    private Area2D _attackArea;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _attackArea = Storage.GetNode<Area2D>("AttackArea");
    }
    private void ToggleAttackArea(bool enable) => _attackArea.Monitoring = enable;
    protected override void Enter()
    {
        _sprite.Play("Summon");
        _enemy.Velocity = Vector2.Zero;
        _timeElapsed = 0;

        GetTree().CreateTimer(LockWait).Timeout += () =>
        {
            if (!IsInstanceValid(_enemy)) return;
            _startAttack = true;
            _lockPos = RandomPosAroundPlayer;
            CallDeferred(MethodName.ToggleAttackArea, true);
        };
    }
    protected override void FrameUpdate(double delta)
    {
        _timeElapsed += (float)delta;
    }
    private void TryAttack()
    {
        if (!_attackArea.Monitoring) return;
        foreach (var body in _attackArea.GetOverlappingBodies())
            if (body is Player)
                _enemy.SendDamageRequest((int)Stats.GetStatValue("Damage"));
    }
    protected override void PhysicsUpdate(double delta)
    {
        if (!_startAttack) return;
        TryAttack();
        if (!LockedOnPlayer)
        {
            Vector2 direction = (_lockPos - EnemyPos).Normalized();
            float distance = EnemyPos.DistanceTo(_lockPos);
            _enemy.Velocity = direction * Stats.GetStatValue("LockSpeed") * distance / 80f;
            if (distance <= 10f)
                LockedOnPlayer = true;
        }
        else
        {
            _enemy.Velocity = _dashDirection * Stats.GetStatValue("LockSpeed") * DashSpeedMultiplier;
        }
    }
    protected override void Exit()
    {
        _startAttack = false;
        LockedOnPlayer = false;
    }
}
