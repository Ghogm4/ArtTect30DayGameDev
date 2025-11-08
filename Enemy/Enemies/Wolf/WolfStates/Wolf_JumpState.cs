using Godot;
using System;

public partial class Wolf_JumpState : State
{
    private EnemyBase _enemy = null;
    private AnimatedSprite2D _sprite = null;
    private Player _player = null;
    private Vector2 _targetPosition;
    [Export] private float _jumpSpeed = 500f; // 跳跃速度

    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _player = GetTree().GetFirstNodeInGroup("Player") as Player;
    }

    protected override void Enter()
    {
        GD.Print("Enter Wolf Jump State");
        _sprite.Play("Jump"); // 假设有 Jump 动画
        _targetPosition = _player.GlobalPosition;
        _enemy.Velocity = (_targetPosition - _enemy.GlobalPosition).Normalized() * _jumpSpeed;
        
        _enemy.Velocity = new Vector2(_enemy.Velocity.X, -300f); 
        Storage.SetVariant("IsJumping", true);
        Storage.SetVariant("IsCharging", false);
    }

    protected override void PhysicsUpdate(double delta)
    {
        Vector2 velocity = _enemy.Velocity;
        if (velocity.Y < -300f) velocity.Y = -300f;
        if (!_enemy.IsOnFloor())
        {
            velocity.Y += _enemy.GetGravity().Y * (float)delta; // 应用重力
            velocity.Y = Mathf.Min(velocity.Y, 1000f); // 限制最大下落速度
        }
        _enemy.Velocity = velocity;
        if (_enemy.GlobalPosition.DistanceTo(_targetPosition) < 10f)
        {

            if (_player != null && _player.GlobalPosition.DistanceTo(_enemy.GlobalPosition) < 20f)
            {
                _player.TakeDamage(1, Callable.From<Player>((player) => { }));
            }
            AskTransit("AttackIdle");
        }

        if (_enemy.IsOnFloor())
        {
            AskTransit("AttackIdle");
        }
    }
    protected override void Exit()
    {
        Storage.SetVariant("IsJumping", false);
    }
}