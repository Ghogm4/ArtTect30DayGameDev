using Godot;
using System;

public partial class Skeleton_AttackState : State
{
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	private Player _player = null;
	private Vector2 _playerLastPosition = Vector2.Zero;

	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<EnemyBase>("Enemy");

		_sprite.AnimationFinished += () =>
		{
			_sprite.Stop();
			AskTransit("AttackIdle");
		};
		Storage.RegisterVariant<bool>("HasDealtDamage1", false);
		Storage.RegisterVariant<bool>("HasDealtDamage2", false);
	}

	

	protected override void Enter()
	{
		GD.Print("Enter Skeleton Attack State");

		// 停止所有动作
		Storage.SetVariant("IsAttackIdling", false);
		Storage.SetVariant("IsAttacking", true);
		Storage.SetVariant("IsRunning", false);
		Storage.SetVariant("HasDealtDamage1", false);
		Storage.SetVariant("HasDealtDamage2", false);
		_enemy.Velocity = new Vector2(0, _enemy.Velocity.Y);
		_sprite.Play("Attack");

		
	}

	protected override void FrameUpdate(double delta)
	{
		if ((_sprite.Frame == 4 || _sprite.Frame == 5) && Storage.GetVariant<bool>("HasDealtDamage1") == false)
		{
			DealDamage1();
			Storage.SetVariant("HasDealtDamage1", true);
		}

		if ((_sprite.Frame == 8 || _sprite.Frame == 9) && Storage.GetVariant<bool>("HasDealtDamage2") == false)
		{
			DealDamage2();
			Storage.SetVariant("HasDealtDamage2", true);
		}
	}
	public void DealDamage1()
	{
		GD.Print("Skeleton Attack 1 Damage Dealt");
		foreach (Node body in _enemy.AttackArea.GetOverlappingBodies())
		{
			if (body is Player player)
			{
				GD.Print("Player Hit by Skeleton Attack 1");
				player.TakeDamage(1, Callable.From<Player>((player) => { }));
			}
		}
	}
	public void DealDamage2()
	{
		GD.Print("Skeleton Attack 2 Damage Dealt");
		foreach (Node body in _enemy.ChaseArea.GetOverlappingBodies())
		{
			if (body is Player player)
			{
				GD.Print("Player Hit by Skeleton Attack 2");
				player.TakeDamage(1, Callable.From<Player>((player) => { }));
			}
		}
	}
}
