using Godot;
using System;

public partial class Archer_IdleState : State
{
    private AnimatedSprite2D _sprite = null;
    private EnemyBase _enemy = null;
    private Player _player = null;

    protected override void ReadyBehavior()
    {
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _enemy = Storage.GetNode<EnemyBase>("Enemy");
        _player = GetTree().GetFirstNodeInGroup("Player") as Player;

        _enemy.Connect("EnterMonitor", new Callable(this, nameof(OnEnterMonitor)));
    }

    protected override void Enter()
    {
        _sprite.Stop();
        _sprite.Play("Idle");
        GD.Print("Enter Idle State");
    }

    public void OnEnterMonitor(Node2D body)
    {
        if (body is Player)
        {
            GD.Print("Player detected in Archer Idle State");
            if (_player == null)
            {
                GD.Print("Player reference is null!");
            }
            AskTransit("Run");
        }
    }

    protected override void Exit()
    {
        _enemy.Disconnect("EnterMonitor", new Callable(this, nameof(OnEnterMonitor)));
        GD.Print("Exit Idle State");
    }
}
