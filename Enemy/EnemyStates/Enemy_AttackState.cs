using Godot;
using System;

public partial class Enemy_AttackState : State
{
	public AnimatedSprite2D _sprite = null;
	private Enemy _enemy = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<Enemy>("Enemy");
		_enemy.Connect("LeaveAttack", new Callable(this, nameof(OnLeaveAttack)));
	}

	protected override void Enter()
	{
		GD.Print("Enemy Entered Attack State");
		_sprite.Play("Attack1");
		_enemy.Velocity = Vector2.Zero;
	}

	protected override void FrameUpdate(double delta)
	{

	}

	protected override void Exit()
	{
	   
	}
	
	public void OnLeaveAttack(Node2D body)
	{
		if (body is Player)
		{
			AskTransit("Chase");
			GD.Print("Transition to Chase State");
		}
	}
}
