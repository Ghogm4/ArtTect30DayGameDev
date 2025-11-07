using Godot;
using System;

public partial class Archer_RollState : State
{
    private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy = null;
    private Player _player = null;

    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _player = GetTree().GetFirstNodeInGroup("Player") as Player;

        _sprite.AnimationFinished += () =>
        {
            _sprite.Stop();
            Storage.SetVariant("IsRolling", false);
            AskTransit("AttackIdle");
        };
    }

    protected override void Enter()
    {
        _sprite.Play("Roll");
        GD.Print("Enter Archer Roll State");

        // 停止奔跑
        Storage.SetVariant("IsAttackIdling", false);
        Storage.SetVariant("IsAttacking", false);
        Storage.SetVariant("IsRunning", false);
    }

    protected override void FrameUpdate(double delta)
    {
        if (Storage.GetVariant<bool>("IsRolling") == false)
        {
            GD.Print("Exit Roll State");
            AskTransit("Attack");
        }
    }
}
