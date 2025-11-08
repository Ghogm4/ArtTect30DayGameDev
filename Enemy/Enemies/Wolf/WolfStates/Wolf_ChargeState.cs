using Godot;
using System;

public partial class Wolf_ChargeState : State
{
    private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy = null;
    private Player _player = null;

    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _player = GetTree().GetFirstNodeInGroup("Player") as Player;

        
    }

    protected override void Enter()
    {
        _sprite.Play("Charge");
        GD.Print("Enter Wolf Charge State");
        Storage.SetVariant("IsCharging", true);
        Storage.SetVariant("IsJumping", false);
        Storage.SetVariant("JumpCooldown", 3f);
        _sprite.AnimationFinished += OnAnimationFinished;
    }
    protected override void Exit()
    {
        _sprite.AnimationFinished -= OnAnimationFinished;
    }

    public void OnAnimationFinished()
    {
        AskTransit("Jump");
        
    }
}
