using Godot;
using System;

public partial class Player_Attack1State : State
{
    [Export] private Timer _attack1ComboTimer = null;
    private AnimatedSprite2D _sprite = null;
    private bool _canCombo = false;
    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _attack1ComboTimer.Timeout += OnAttack1ComboTimerTimeout;
    }
    protected override void Enter()
    {
        _sprite.Play("Attack1");
        _attack1ComboTimer.Start();
    }
    protected override void FrameUpdate(double delta)
    {
        if (_canCombo && Input.IsActionJustPressed("Attack"))
            AskTransit("Attack2");
    }
    protected override void Exit()
    {
        _canCombo = false;
        _attack1ComboTimer.Stop();
    }

    private void OnAttack1ComboTimerTimeout() => _canCombo = true;
}
