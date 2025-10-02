using Godot;
using System;

public partial class Player_JumpState : State
{
    [Export] private Timer _riseToFallTimer = null;
    private AnimatedSprite2D _sprite = null;
    private Player _player = null;

    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _player = Storage.GetNode<Player>("Player");
        _riseToFallTimer.Timeout += OnRiseToFallTimerTimeout;
    }
    protected override void Enter()
    {
        _sprite.Play("Rise");
        _riseToFallTimer.Start();
    }
    protected override void FrameUpdate(double delta)
    {
        if (_player.IsOnFloor())
            AskTransit("Idle");
    }
    private void OnRiseToFallTimerTimeout()
    {
        _sprite.Play("Fall");
    }
    protected override void Exit()
    {
        _riseToFallTimer.Stop();
    }
}
