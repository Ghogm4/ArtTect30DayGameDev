using Godot;
using System;

public partial class Wolf_RunState : State
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
		Storage.SetVariant("IsRunning", true);
		_sprite.Stop();
		_sprite.Play("Run");
		GD.Print("Enter Wolf Run State");
	}

    protected override void PhysicsUpdate(double delta)
    {
        if (Storage.GetVariant<bool>("IsAttacking"))
        {
            GD.Print("Transition to Attack State from Run State");
            AskTransit("Attack");
        }
    }

    protected override void FrameUpdate(double delta)
    {
        if (_enemy.Velocity.X <= 10f)
        {
            if (_sprite.Animation != "Idle")
            {
                GD.Print("Idle");
                _sprite.Stop();
                _sprite.Play("Idle");
            }
            
        }
        else
        {
            if (_sprite.Animation != "Run")
            {
                _sprite.Stop();
                _sprite.Play("Run");
            }
            
        }
    }

    protected override void Exit()
    {
        Storage.SetVariant("IsRunning", false);
    }
}
