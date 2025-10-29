using Godot;
using System;

public partial class GuardianOfTheForest_DieState : State
{
    [Export] public RayCast2D GroundDetect;
    [Export] public float DeathExplosionRadius = 100f;
    [Export] public float DeathExplosionInterval = 0.1f;
    [Export] public float MaxFallSpeed = 500f;
    [Export] public PackedScene DeathExplosionEffectScene;
    private AnimationPlayer _animationPlayer = null;
    private EnemyBase _enemy = null;
    private bool _falling = false;
    private Vector2 GetRandomPosition(Vector2 pivot)
    {
        Vector2 offset = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau)) * (float)GD.RandRange(0, DeathExplosionRadius);
        return pivot + offset;
    }
    protected override void ReadyBehavior()
    {
        _animationPlayer = Storage.GetNode<AnimationPlayer>("AnimationPlayer");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
    }
    protected override void Enter()
    {
        _animationPlayer.Play("Die");
        _enemy.Velocity = Vector2.Zero;
        Storage.SetVariant("IsInDieState", true);
    }
    protected override void PhysicsUpdate(double delta)
    {
        if (!_falling) return;
        if (GroundDetect.IsColliding())
        {
            _animationPlayer.Play(_animationPlayer.AssignedAnimation);
            _falling = false;
            GroundDetect.Enabled = false;
            _enemy.Velocity = Vector2.Zero;
            return;
        }
        Vector2 velocity = _enemy.Velocity;
        velocity.Y = Mathf.Min(velocity.Y + _enemy.GetGravity().Y * (float)delta, MaxFallSpeed);
        _enemy.Velocity = velocity;
        _enemy.MoveAndSlide();
    }
    private void OnAnimationNearEnd()
    {
        _enemy.Die(false);
        _animationPlayer.Pause();
        _animationPlayer.SpeedScale = 0;
    }
    private void SpawnDeathExplosionEffect()
    {
        Vector2 randomPos = GetRandomPosition(_enemy.GlobalPosition);
        Node2D effect = DeathExplosionEffectScene.Instantiate<Node2D>();
        effect.GlobalPosition = randomPos;
        GetTree().CurrentScene.AddChild(effect);
    }
    private void EnableFalling()
    {
        _falling = true;
        _animationPlayer.Pause();
        GroundDetect.Enabled = true;
    }
}
