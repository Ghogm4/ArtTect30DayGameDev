using Godot;
using System;

public partial class RedSlime_MoveControlState : State
{
	private EnemyBase _enemy = null;
	private Player _player = null;
	private AnimatedSprite2D _sprite = null;
	[Export] private float _maxFallSpeed = 500f;
	// [Export] public float Speed = 105f;
	private StatWrapper _speed;
	// [Export] public float JumpForce = 120f;
	private StatWrapper _jumpForce;
	// [Export] public float Damage = 2f;
	private StatWrapper _damage;

	[Signal] public delegate void JumpEventHandler();
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");

		Storage.RegisterVariant<bool>("Is_Chasing", false);
		Storage.RegisterVariant<bool>("HeadingLeft", false);
		Storage.RegisterVariant<bool>("Is_Jumping", false);
		Storage.RegisterVariant<bool>("Is_Collision", false);
		Storage.RegisterVariant<bool>("Colliding", false);

		_speed = new(Stats.GetStat("Speed"));
		_jumpForce = new(Stats.GetStat("JumpForce"));
		_damage = new(Stats.GetStat("Damage"));

		
		var jumpState = GetNode<State>("Jump");
		jumpState.Connect("Jump", new Callable(this, nameof(OnJump)));
	}
	protected override void PhysicsUpdate(double delta)
	{
		Vector2 velocity = _enemy.Velocity;
		if (!_enemy.IsOnFloor())
			velocity.Y = Math.Min(_enemy.GetGravity().Y * (float)delta * 0.5f + velocity.Y, _maxFallSpeed);


		if (Storage.GetVariant<bool>("Is_Chasing") && !_enemy.IsOnFloor() && !Storage.GetVariant<bool>("Is_Collision"))
		{
			velocity.X = (_player.GlobalPosition.X < _enemy.GlobalPosition.X) ? -(float)_speed : (float)_speed;
			Storage.SetVariant("HeadingLeft", velocity.X < 0);
		}

		else
		{
			velocity.X = 0;
		}

		if (Storage.GetVariant<bool>("Is_Collision"))
		{
			
			velocity = new Vector2((_player.GlobalPosition.X < _enemy.GlobalPosition.X) ? -(float)_speed : (float)_speed, velocity.Y);
		}

		if (Storage.GetVariant<bool>("Colliding"))
		{
			
			velocity = new Vector2(velocity.X, -20f);
			
		}
		_enemy.Velocity = velocity;
	}

	protected override void FrameUpdate(double delta)
	{
		bool headingLeft = Storage.GetVariant<bool>("HeadingLeft");
		if (headingLeft)
			_sprite.FlipH = true;
		else
			_sprite.FlipH = false;

		if (Storage.GetVariant<bool>("Colliding"))
		{
			Storage.SetVariant("Colliding", false);
			Attack();
		}
	}

	public void OnJump()
	{
		Vector2 velocity = _enemy.Velocity;
		velocity.Y = -(float)_jumpForce;
		_enemy.Velocity = velocity;
		Storage.SetVariant("Is_Jumping", true);
	}
	public void Attack()
	{
		_enemy.SendDamageRequest((float)_damage);
	}
}
