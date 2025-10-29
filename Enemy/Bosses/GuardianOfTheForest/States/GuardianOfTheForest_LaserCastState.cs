using Godot;
using System;

public partial class GuardianOfTheForest_LaserCastState : State
{
    [Export] public Laser AttackLaser = null;
    [Export] public float MinRotationSpeed = 1f;
    [Export] public float MaxRotationSpeed = 2.5f;
    [Export] public float StateDuration = 5.0f;
    private float Health => Stats.GetStatValue("Health");
    private float MaxHealth => Stats.GetStatValue("MaxHealth");
    private float Ratio => Mathf.Clamp(Health / MaxHealth, 0, 1);
    private float RotationSpeed => Mathf.Lerp(MaxRotationSpeed, MinRotationSpeed, Ratio);
    private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy = null;
    private Player _player = null;
    private bool _isFollowingPlayer = false;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _player = GetTree().GetFirstNodeInGroup("Player") as Player;
    }
    protected override void Enter()
    {
        _sprite.Play("LaserCast");
        AttackLaser.Appearing = true;
        _enemy.Velocity = Vector2.Zero;
        GetTree().CreateTimer(StateDuration).Timeout += () =>
        {
            if (_enemy.IsDead)
                return;
            AttackLaser.Appearing = false;
            AskTransit("Normal");
        };
        _sprite.AnimationFinished += OnAnimationFinished;
        Vector2 direction = (_player.GlobalPosition - AttackLaser.GlobalPosition).Normalized();
        float angle = direction.Angle();
        AttackLaser.Rotation = angle;
    }
    protected override void Exit()
    {
        AttackLaser.Appearing = false;
        _sprite.AnimationFinished -= OnAnimationFinished;
        _isFollowingPlayer = false;
    }
    protected override void FrameUpdate(double delta)
    {
        if (!_isFollowingPlayer)
            return;
        Vector2 direction = (_player.GlobalPosition - AttackLaser.GlobalPosition).Normalized();
        float angle = direction.Angle();
        float angleDiff = Mathf.AngleDifference(AttackLaser.Rotation, angle);
        float rotationStep = RotationSpeed * (float)delta;

        if (Mathf.Abs(angleDiff) <= rotationStep)
            AttackLaser.Rotation = angle;
        else
            AttackLaser.Rotation += Mathf.Sign(angleDiff) * rotationStep;
    }
    private void OnAnimationFinished()
    {
        _isFollowingPlayer = true;
    }
}
