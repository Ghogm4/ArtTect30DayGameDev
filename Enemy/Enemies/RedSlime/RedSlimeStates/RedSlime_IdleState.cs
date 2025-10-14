using Godot;
using System;

public partial class RedSlime_IdleState : State
{
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	private CharacterBody2D _enemy = null;
	
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		_enemy = Storage.GetNode<CharacterBody2D>("Enemy");

		_enemy.Connect("EnterMonitor", new Callable(this, nameof(OnEnterMonitor)));
	}

	protected override void Enter()
	{
		_sprite.Stop();
		_sprite.Play("Idle");
	}

	public void OnEnterMonitor(Node2D body)
	{
		if (body is Player)
		{
			Storage.SetVariant("Is_Chasing", true);
			if (_player == null)
			{
				GD.Print("Player reference is null!");
			}
			_sprite.Stop();
			AskTransit("Jump");
		}
	}

}
