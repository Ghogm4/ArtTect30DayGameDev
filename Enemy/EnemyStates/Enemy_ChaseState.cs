using Godot;
using System;

public partial class Enemy_ChaseState : State
{
	public AnimatedSprite2D _sprite = null;
	private Enemy _enemy = null;

	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<Enemy>("Enemy");
	}
	protected override void Enter()
	{
		_sprite.Play("Run");

	}

	protected override void FrameUpdate(double delta)
	{
		Vector2 playerPos = Storage.GetVariant<Vector2>("PlayerPosition");
		Vector2 direction = (playerPos - _enemy.GlobalPosition).Normalized();
		_enemy.Velocity = direction * _enemy.ChaseSpeed;
		_enemy.MoveAndSlide();
		if (direction.X < 0)
		{
			Storage.SetVariant("HeadingLeft", true);
			_sprite.FlipH = true;
		}
		else
		{
			_sprite.FlipH = false;
		}
		float distance = _enemy.GlobalPosition.DistanceTo(playerPos);
		if (distance > 300f)
		{
			AskTransit("Idle");
		}
	}
	
	protected override void Exit()
    {
        _enemy.Velocity = Vector2.Zero;
    }
}