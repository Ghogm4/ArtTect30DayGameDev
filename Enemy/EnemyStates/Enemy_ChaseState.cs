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

		_enemy.Connect("EnterAttack", new Callable(this, nameof(OnEnterAttack)));
		_sprite.AnimationFinished += () =>
        {
            AskTransit("Idle");
        };
	}
	protected override void Enter()
	{
		_sprite.Play("Run");

	}

	protected override void FrameUpdate(double delta)
	{
		Vector2 playerPos = Storage.GetVariant<Vector2>("PlayerPosition");
		Vector2 direction = (playerPos - _enemy.GlobalPosition).Normalized();
		_enemy.Velocity = new Vector2(direction.X * _enemy.ChaseSpeed, 0);
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
		if (distance > 200f)
		{
			AskTransit("Idle");
		}
	}

	protected override void PhysicsUpdate(double delta)
	{
		Vector2 velocity = _enemy.Velocity;
		if (!_enemy.IsOnFloor())
			velocity.Y = Math.Min(_enemy.GetGravity().Y * (float)delta * 0.5f + velocity.Y, _enemy.MaxFallSpeed);
		_enemy.Velocity = velocity;
	}
	
	public void OnEnterAttack(Node2D body)
	{
		if (body is Player)
		{
			AskTransit("Attack");
			GD.Print("Transition to Idle State");
		}
	}
	protected override void Exit()
    {
        _enemy.Velocity = Vector2.Zero;
    }
}