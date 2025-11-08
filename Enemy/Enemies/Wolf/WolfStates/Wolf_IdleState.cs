using Godot;
using System;

public partial class Wolf_IdleState : State
{
	private EnemyBase _enemy = null;
	private Player _player = null;
	private AnimatedSprite2D _sprite = null;

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
		GD.Print("Enter Wolf Idle State");
	}

	public void OnEnterMonitor(Node2D body)
	{
		if (body is Player)
		{
			GD.Print("Player detected in Skeleton Idle State");
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
