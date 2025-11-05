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

	[Export] private float _headHorizontalThreshold = 16f;
	[Export] private float _headVerticalThreshold = 8f;

	// mid-jump horizontal lock
	[Export] private float _midJumpLockFraction = 0.5f;
	private float _initialJumpSpeed = 0f;
	private bool _midJumpLocked = false;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");

		Storage.RegisterVariant<bool>("Is_Chasing", false);
		Storage.RegisterVariant<bool>("HeadingLeft", false);
		Storage.RegisterVariant<bool>("Is_Jumping", false);
		Storage.RegisterVariant<bool>("On_Player_Head", false);
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
		bool wasJumping = Storage.GetVariant<bool>("Is_Jumping");
		if (!_enemy.IsOnFloor())
			velocity.Y = Math.Min(_enemy.GetGravity().Y * (float)delta * 0.5f + velocity.Y, _maxFallSpeed);
		else
		{
			// landed: clear mid-jump lock
			_midJumpLocked = false;
			// landing event: if we were jumping, check if landed on player's head
			if (wasJumping)
			{
				float dx = Math.Abs(_enemy.GlobalPosition.X - _player.GlobalPosition.X);
				bool isAbove = _enemy.GlobalPosition.Y + _headVerticalThreshold < _player.GlobalPosition.Y;
				if (isAbove && dx <= _headHorizontalThreshold)
				{
					Storage.SetVariant("On_Player_Head", true);
				}
			}
			// clear jumping flag on floor
			Storage.SetVariant("Is_Jumping", false);
		}

		// if currently jumping and not yet locked, detect midpoint by comparing vertical speed
		if (Storage.GetVariant<bool>("Is_Jumping") && !_midJumpLocked && !_enemy.IsOnFloor())
		{
			// ascending only (velocity.Y < 0)
			if (velocity.Y < 0 && _initialJumpSpeed > 0f)
			{
				if (Mathf.Abs(velocity.Y) <= _initialJumpSpeed * Mathf.Clamp(_midJumpLockFraction, 0f, 1f))
				{
					_midJumpLocked = true;
				}
			}
		}

		// Clear On_Player_Head if slime moved away from player's head
		if (Storage.GetVariant<bool>("On_Player_Head"))
		{
			float dxCheck = Math.Abs(_enemy.GlobalPosition.X - _player.GlobalPosition.X);
			bool stillAbove = _enemy.GlobalPosition.Y + _headVerticalThreshold < _player.GlobalPosition.Y && dxCheck <= _headHorizontalThreshold;
			if (!stillAbove)
				Storage.SetVariant("On_Player_Head", false);
		}

		// Only update horizontal velocity if we are NOT mid-jump-locked and NOT sitting on player's head
		if (!(_midJumpLocked && Storage.GetVariant<bool>("Is_Jumping")) && !Storage.GetVariant<bool>("On_Player_Head"))
		{
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
		// record initial upward speed for mid-jump detection and reset lock
		_initialJumpSpeed = (float)_jumpForce;
		_midJumpLocked = false;
	}
	public void Attack()
	{
		_enemy.SendDamageRequest((float)_damage);
	}
}
