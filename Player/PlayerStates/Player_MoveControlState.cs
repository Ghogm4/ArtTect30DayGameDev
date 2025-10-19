using Godot;
using System;
using System.IO;
using System.Threading.Tasks;

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
	private bool _wasOnFloor = false;
	private float _platformVelocity = 0f;
	private bool _jumpedFromPlatform = false;
	private float _launchPlatformSpeed = 0f;
	private const float _airDragMin = 0.1f;
	private const float _airDragMax = 10f;
	private const float _platformSpeedMax = 200f;
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
		{
			ResetJumps();
			_platformVelocity = _player.GetPlatformVelocity().X;
			_wasOnFloor = true;

			_jumpedFromPlatform = false;
			_launchPlatformSpeed = 0f;
		}
		else if (_wasOnFloor)
		{
			velocity += new Vector2(_platformVelocity, 0);
			_wasOnFloor = false;
		}

		if (Input.IsActionPressed("Down") && Input.IsActionJustPressed("Jump") && _isOnFloor)
			PassThroughPlatform();
		
		if (!Input.IsActionPressed("Down") &&
			(
				(Input.IsActionJustPressed("Jump") && AvailableJumps > 0) ||
				(_willJump && _isOnFloor)
			))
		{
			velocity.Y = -Stats.GetStatValue("JumpVelocity");
			_willJump = false;
			_shortJumpTimer.Start();
			AvailableJumps--;
			CanCoyoteTimerStart = false;

			_jumpedFromPlatform = _wasOnFloor && Mathf.Abs(_platformVelocity) > 0.1f;
			if (_jumpedFromPlatform) _launchPlatformSpeed = Mathf.Abs(_platformVelocity);
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
		if (!_isOnFloor)
		{
			if (direction != 0 && Mathf.Abs(velocity.X) <= Speed)
			{
				velocity.X = Mathf.Clamp(direction * Acceleration + velocity.X, -Speed, Speed);
			}
			else
			{
				float drag = _airDragMax;
				if (_jumpedFromPlatform)
				{
					float t = Mathf.Clamp(_launchPlatformSpeed / _platformSpeedMax, 0f, 1f);
					drag = Mathf.Lerp(_airDragMax, _airDragMin, t);
				}
				velocity.X = Mathf.Lerp(velocity.X, 0, drag * (float)delta);

			}
		}
		else
		{
			if (direction != 0)
			{
				velocity.X = Mathf.Clamp(direction * Acceleration + velocity.X, -Speed, Speed);
			}
			else
			{
				velocity.X = Mathf.MoveToward(velocity.X, 0, Deceleration);
			}
		}
		if (direction != 0)
			Storage.SetVariant("HeadingLeft", direction < 0);

		_player.Velocity = velocity;
		_player.MoveAndSlide();
	}
	private async void PassThroughPlatform()
	{
		_player.SetCollisionMaskValue(5, false);
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
		_player.SetCollisionMaskValue(5, true);
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
