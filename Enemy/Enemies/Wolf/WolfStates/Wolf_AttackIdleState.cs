using Godot;
using System;

public partial class Wolf_AttackIdleState : State
{
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	private Timer _attackIdleTimer = null;
	private Player _player = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;

		_attackIdleTimer = new Timer();
		GetTree().Root.CallDeferred(MethodName.AddChild, _attackIdleTimer);
		_attackIdleTimer.WaitTime = 0.5f; // 攻击间隔时间
		_attackIdleTimer.OneShot = true;
		_attackIdleTimer.Timeout += () =>
		{
			Storage.SetVariant("IsAttackIdling", false);
			Storage.SetVariant("IsAttacking", true);
			AskTransit("Attack");
		};
	}
	protected override void Enter()
	{
		GD.Print("Enter Wolf Attack Idle State");
		_sprite.Play("Idle");
		_attackIdleTimer.Start();
		Storage.SetVariant("IsRunning", false);
		Storage.SetVariant("IsAttacking", false);
		Storage.SetVariant("IsAttackIdling", true);
		Storage.SetVariant("IsJumping", false);
		Storage.SetVariant("IsCharging", false);
	}

	protected override void PhysicsUpdate(double delta)
	{
		if (Storage.GetVariant<bool>("IsRunning") == true)
		{
			_attackIdleTimer.Stop();
			AskTransit("Run");
		}
		
	}
}
