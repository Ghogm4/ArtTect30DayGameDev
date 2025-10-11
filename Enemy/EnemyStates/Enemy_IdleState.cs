using Godot;
using System;

public partial class Enemy_IdleState : State
{
	public AnimatedSprite2D _sprite = null;
	private Enemy _enemy = null;

	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<Enemy>("Enemy");

		_enemy.Connect("EnterMonitor", new Callable(this, nameof(OnEnterMonitor)));
	}

	protected override void Enter()
	{
		GD.Print("Enemy Entered Idle State");
		_sprite.Play("Idle");
	}
	protected override void FrameUpdate(double delta)
	{

	}
	
	public void OnEnterMonitor(Node2D body)
	{
		if (body is Player)
		{
			AskTransit("Chase");
			GD.Print("Transition to Chase State");
		}
	}
}
