using Godot;
using System;

public partial class LargeBat_NormalState : State
{
    [Export] public float WanderRadius = 100f;
    [Export] public float ChaseRadius = 50f;
    [Export] public float Duration = 200f;
    private Vector2 RandomWanderPos
    {
        get
        {
            float wanderOffset = (float)GD.RandRange(0, WanderRadius);
            Vector2 wanderDirection = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
            return _originalGlobalPos + wanderDirection * wanderOffset;
        }
    }
    private Vector2 RandomChasePos
    {
        get
        {
            float chaseOffset = (float)GD.RandRange(0, ChaseRadius);
            Vector2 chaseDirection = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
            return _player.GlobalPosition + chaseDirection * chaseOffset;
        }
    }
    private float MoveSpeed
    {
        get => _player == null ? Stats.GetStatValue("Speed") : Stats.GetStatValue("ChaseSpeed");
    }
    private Player _player = null;
    private Area2D _detectArea;
    private AnimatedSprite2D _sprite;
    private EnemyBase _enemy;
    private Vector2 _originalGlobalPos;
    private Vector2 _targetMovePos;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _detectArea = Storage.GetNode<Area2D>("DetectArea");
        _detectArea.BodyEntered += OnBodyEntered;
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _originalGlobalPos = _enemy.GlobalPosition;
    }
    private void CreateTransitionTimer()
    {
        GetTree().CreateTimer(Duration).Timeout += () =>
        {
            if (IsInstanceValid(this))
                AskTransit("Decision");
        };
    }
    protected override void Enter()
    {
        _sprite.Play("Normal");
        if (_player is null)
        {
            _targetMovePos = RandomWanderPos;
        }
        else
        {
            _targetMovePos = RandomChasePos;
            CreateTransitionTimer();
        }
    }

    protected override void PhysicsUpdate(double delta)
    {
        DecideTargetPos();
        MoveToTargetPos();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player && _player == null)
        {
            _player = player;
            _targetMovePos = RandomChasePos;
            Storage.RegisterNode("Player", _player);
            CreateTransitionTimer();
        }
    }
    private void MoveToTargetPos()
    {
        Vector2 direction = (_targetMovePos - _enemy.GlobalPosition).Normalized();
        _enemy.Velocity = direction * MoveSpeed;
    }
    private void DecideTargetPos()
    {
        if (_enemy.GlobalPosition.DistanceTo(_targetMovePos) > 3f) return;
        _targetMovePos = _player == null ? RandomWanderPos : RandomChasePos;
    }
}
