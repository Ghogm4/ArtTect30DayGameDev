using Godot;
using System;

public partial class Skeleton_DieState : State
{
	private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy = null;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");

        _sprite.AnimationFinished += () =>
        {
            _sprite.Stop();
            GD.Print("Skeleton Die Animation Finished");
        };
    }
    protected override void Enter()
    {
        //_sprite.Play("Die");
        GD.Print("Enter Archer Die State");

        // 停止所有动作
        //Storage.SetVariant("IsAttackIdling", false);
        Storage.SetVariant("IsAttacking", false);
		Storage.SetVariant("IsRunning", false);
		Storage.SetVariant("IsAttackIdling", false);
        
        _enemy.Velocity = Vector2.Zero;
    }
}
