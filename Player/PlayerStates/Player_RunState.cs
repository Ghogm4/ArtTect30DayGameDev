using Godot;
using System;

public partial class Player_RunState : State
{
    private AnimatedSprite2D _sprite = null;
    private Player _player = null;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _player = Storage.GetNode<Player>("Player");
    }
    protected override void Enter()
    {
        _sprite.Play("Run");
    }
    protected override void FrameUpdate(double delta)
    {
        if (!_player.IsOnFloor())
			AskTransit("Jump");
        if (Mathf.IsZeroApprox(_player.Velocity.X))
            AskTransit("Idle");
    }
}
