using Godot;
using System;

public partial class GuardianOfTheForest_LaserCastState : State
{
    [Export] public Laser AttackLaser = null;
    [Export] public float MinRotationSpeed = 1f;
    [Export] public float MaxRotationSpeed = 2.5f;
    [Export] public float StateDuration = 5.0f;
    [Export] public float BackBlueOrbSpeed = 200f;
    [Export] public float BackOrbSpawnInterval = 0.5f;
    [Export] public int BackOrbSpawnCount = 3;
    [Export] public float BackOrbSpawnSpreadRadian = 0.5f;
    [Export] public float MoveSpeed = 80f;
    [Export] public PackedScene BlueOrbScene = null;
    private float Health => Stats.GetStatValue("Health");
    private float MaxHealth => Stats.GetStatValue("MaxHealth");
    private float Ratio => Mathf.Clamp(Health / MaxHealth, 0, 1);
    private float RotationSpeed => Mathf.Lerp(MaxRotationSpeed, MinRotationSpeed, Ratio);
    private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy = null;
    private Player _player = null;
    private bool _isFollowingPlayer = false;
    private int clockwise = 1;
    private float _timeElapsed = 0f;
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
        Vector2 direction = (_player.GlobalPosition - AttackLaser.GlobalPosition).Normalized();
        float angle = direction.Angle();
        AttackLaser.Rotation = angle;
        GetTree().CreateTimer(StateDuration).Timeout += () =>
        {
            if (_enemy.IsDead)
                return;
            AttackLaser.Appearing = false;
            AskTransit("Decision");
        };
        _sprite.AnimationFinished += OnAnimationFinished;
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
        AttackLaser.Rotation += clockwise * RotationSpeed * (float)delta;
        _timeElapsed += (float)delta;
        if (_timeElapsed >= BackOrbSpawnInterval)
        {
            _timeElapsed -= BackOrbSpawnInterval;
            float radian = AttackLaser.Rotation + Mathf.Pi;
            for (int i = 0; i < BackOrbSpawnCount; i++)
                SpawnOrb(BackBlueOrbSpeed, radian + (float)GD.RandRange(-BackOrbSpawnSpreadRadian, BackOrbSpawnSpreadRadian), false);
        }
        Vector2 direction = (_player.GlobalPosition - AttackLaser.GlobalPosition).Normalized();
        _enemy.Velocity = direction * MoveSpeed;
        _enemy.MoveAndSlide();
    }
    private void OnAnimationFinished()
    {
        _isFollowingPlayer = true;
        Vector2 direction = (_player.GlobalPosition - AttackLaser.GlobalPosition).Normalized();
        float angle = direction.Angle();
        float angleDiff = Mathf.AngleDifference(AttackLaser.Rotation, angle);
        clockwise = Mathf.IsZeroApprox(angleDiff) ? Probability.RunUniformChoose([-1, 1]) : Mathf.Sign(angleDiff);
    }
    private void SpawnOrb(float speed, float radian, bool canPierceWorld)
    {
        var orb = BlueOrbScene.Instantiate<BlueOrb>();
        orb.GlobalPosition = AttackLaser.GlobalPosition;
        orb.Velocity = Vector2.Right.Rotated(radian) * speed;
        orb.Rotation = orb.Velocity.Angle();
        orb.PierceWorld = canPierceWorld;
        GetTree().CurrentScene.CallDeferred(MethodName.AddChild, orb);
    }
}
