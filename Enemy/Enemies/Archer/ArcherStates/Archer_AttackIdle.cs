using Godot;
using System;

public partial class Archer_AttackIdle : State
{
	private EnemyBase _enemy = null;
	private AnimatedSprite2D _sprite = null;
	private Timer _attackIdleTimer = null;
	private Player _player = null;

	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;

		_attackIdleTimer = new Timer();
		GetTree().Root.AddChild(_attackIdleTimer);
		_attackIdleTimer.WaitTime = 3f; // 攻击间隔时间
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
		_sprite.Play("Idle");
		_attackIdleTimer.Start();
		Storage.SetVariant("IsRunning", false);
		Storage.SetVariant("IsAttacking", false);
		Storage.SetVariant("IsAttackIdling", true);
	}

	protected override void PhysicsUpdate(double delta)
	{
		if (Storage.GetVariant<bool>("IsRunning") == true)
		{
			_attackIdleTimer.Stop();
			AskTransit("Run");
		}
		
		if (Storage.GetVariant<bool>("IsRolling"))
		{
			_attackIdleTimer.Stop();
			AskTransit("Roll");
		}
	}
}
