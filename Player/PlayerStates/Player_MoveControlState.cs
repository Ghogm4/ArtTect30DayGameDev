using Godot;
using System;
using System.IO;

public partial class Player_MoveControlState : State
{
	[Export] private Timer _jumpBufferTimer = null;
	[Export] private Timer _coyoteTimer = null;
	[Export] private Timer _shortJumpTimer = null;
	public const float Speed = 150.0f;
	public const float JumpVelocity = 220.0f;
	public const float Acceleration = 25.0f;
	public const float Deceleration = 25.0f;
	public int AvailableJumps => Storage.GetVariant<int>("AvailableJumps");
	private Player _player = null;
	private bool _isOnFloor = false;
	private bool _isCoyoteTimerRunning = false;
	private bool _canCoyoteTimerStart = true;
	private bool _willJump = false;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_coyoteTimer.Timeout += () =>
		{
			if (AvailableJumps == GameData.Instance.PlayerMaxJumps) DecreaseJump();
			_isCoyoteTimerRunning = false;
			_canCoyoteTimerStart = false;
		};
		_jumpBufferTimer.Timeout += () => _willJump = false;
		ResetJumps();
	}
	protected override void PhysicsUpdate(double delta)
	{
		GD.Print(AvailableJumps);
		GD.Print(_coyoteTimer.TimeLeft);
		Vector2 velocity = _player.Velocity;
		_isOnFloor = _player.IsOnFloor();
		HandleCoyoteTime();
		HandleJumpBuffer();

		if (_isOnFloor)
			ResetJumps();
		else
			velocity += _player.GetGravity() * (float)delta * 0.5f;

		if ((Input.IsActionJustPressed("Jump") && AvailableJumps > 0) || (_willJump && _isOnFloor))
		{
			velocity.Y = -JumpVelocity;
			_willJump = false;
			_shortJumpTimer.Start();
			DecreaseJump();
			_canCoyoteTimerStart = false;
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
	private void HandleCoyoteTime()
	{
		if (_isCoyoteTimerRunning) return;
		if (_isOnFloor)
		{
			_canCoyoteTimerStart = true;
		}
		else if (_canCoyoteTimerStart)
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
	private void ResetJumps() => Storage.SetVariant("AvailableJumps", GameData.Instance.PlayerMaxJumps);
	private void DecreaseJump() => Storage.SetVariant("AvailableJumps", AvailableJumps - 1);
	private void IncreaseJump() => Storage.SetVariant("AvailableJumps", AvailableJumps + 1);
}
