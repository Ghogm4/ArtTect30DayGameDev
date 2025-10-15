using Godot;
using System;
using System.IO;

public partial class Player_MoveControlState : State
{
	[Export] private Timer _jumpBufferTimer = null;
	[Export] private Timer _coyoteTimer = null;
	[Export] private Timer _shortJumpTimer = null;
	private const float Acceleration = 25.0f;
	private const float Deceleration = 25.0f;
	private int AvailableJumps
	{
		get => (int)Stats.GetStatValue("AvailableJumps");
		set => Stats.GetStat("AvailableJumps").AddFinal(value - (int)Stats.GetStatValue("AvailableJumps"));
	}
	private float Speed => Stats.GetStatValue("Speed");
	private Player _player = null;
	private bool _isOnFloor = false;
	private bool _isCoyoteTimerRunning = false;
	private bool CanCoyoteTimerStart
	{
		get => Storage.GetVariant<bool>("CanCoyoteTimerStart");
		set => Storage.SetVariant("CanCoyoteTimerStart", value);
	}
	private bool _willJump = false;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_coyoteTimer.Timeout += () =>
		{
			if (AvailableJumps == (int)Stats.GetStatValue("MaxJump"))
				AvailableJumps--;
			_isCoyoteTimerRunning = false;
			CanCoyoteTimerStart = false;
		};
		_jumpBufferTimer.Timeout += () => _willJump = false;
		ResetJumps();
	}
	protected override void PhysicsUpdate(double delta)
	{
		Vector2 velocity = _player.Velocity;
		_isOnFloor = _player.IsOnFloor();
		HandleCoyoteTime();
		HandleJumpBuffer();

		if (_isOnFloor)
			ResetJumps();

		if ((Input.IsActionJustPressed("Jump") && AvailableJumps > 0) || (_willJump && _isOnFloor))
		{
			velocity.Y = -Stats.GetStatValue("JumpVelocity");
			_willJump = false;
			_shortJumpTimer.Start();
			AvailableJumps--;
			CanCoyoteTimerStart = false;
		}
		if (Input.IsActionJustReleased("Jump") && !_shortJumpTimer.IsStopped())
		{
			velocity.Y *= 0.5f;
		}
		if (Input.IsActionPressed("Right"))
			Storage.SetVariant("Direction", 1);
		else if (Input.IsActionPressed("Left"))
			Storage.SetVariant("Direction", -1);
		else
			Storage.SetVariant("Direction", 0);

		int direction = Storage.GetVariant<int>("Direction");
		if (direction != 0)
			velocity.X = Mathf.Clamp(direction * Acceleration + velocity.X, -Speed, Speed);
		else
			velocity.X = Mathf.MoveToward(velocity.X, 0, Deceleration);

		if (direction != 0)
			Storage.SetVariant("HeadingLeft", direction < 0);

		_player.Velocity = velocity;
		_player.MoveAndSlide();
	}
	protected override void Exit()
	{
		_coyoteTimer.Stop();
	}
	private void HandleCoyoteTime()
	{
		if (_isCoyoteTimerRunning) return;
		if (_isOnFloor)
		{
			CanCoyoteTimerStart = true;
		}
		else if (CanCoyoteTimerStart)
		{
			_coyoteTimer.Start();
			_isCoyoteTimerRunning = true;
		}
	}
	private void HandleJumpBuffer()
	{
		if (Input.IsActionJustPressed("Jump") && !_isOnFloor)
		{
			_willJump = true;
			_jumpBufferTimer.Start();
		}
	}
	private void ResetJumps() => AvailableJumps = (int)Stats.GetStatValue("MaxJump");
}
