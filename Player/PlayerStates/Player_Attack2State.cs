using Godot;
using System;

public partial class Player_Attack2State : State
{
    [Export] private Timer _attack2ComboTimer = null;
    private AnimatedSprite2D _sprite = null;
    private bool _canCombo = false;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _attack2ComboTimer.Timeout += OnAttack2ComboTimerTimeout;
    }
    protected override void Enter()
    {
        _sprite.Play("Attack2");
        _attack2ComboTimer.Start();
    }
    protected override void FrameUpdate(double delta)
    {
        if (_canCombo && Input.IsActionJustPressed("Attack"))
            AskTransit("Attack3");
    }
    protected override void Exit()
    {
        _canCombo = false;
        _attack2ComboTimer.Stop();
    }

    private void OnAttack2ComboTimerTimeout() => _canCombo = true;
}
