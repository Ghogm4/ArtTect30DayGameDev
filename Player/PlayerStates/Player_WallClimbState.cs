using Godot;
using System;

public partial class Player_WallClimbState : State
{
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	private RayCast2D _raycastBottomLeft = null;
	private RayCast2D _raycastBottomRight = null;
	private RayCast2D _raycastTopLeft = null;
	private RayCast2D _raycastTopRight = null;

	private int AvailableJumps
	{
		get => (int)Stats.GetStatValue("AvailableJumps");
		set => Stats.GetStat("AvailableJumps").AddFinal(value - (int)Stats.GetStatValue("AvailableJumps"));
	}
	private int AvailableDashes
	{
		get => (int)Stats.GetStatValue("AvailableDashes");
		set => Stats.GetStat("AvailableDashes").AddFinal(value - (int)Stats.GetStatValue("AvailableDashes"));
	}
	private bool HeadingLeft
	{
		get => Storage.GetVariant<bool>("HeadingLeft");
		set => Storage.SetVariant("HeadingLeft", value);
	}
	private bool CanCoyoteTimerStart
	{
		set => Storage.SetVariant("CanCoyoteTimerStart", value);
	}
	private float WallSlideVelocityMultiplier
	{
		get => Stats.GetStatValue("WallSlideVelocityMultiplier");
	}
	private float WallClimbVelocityMultiplier
	{
		get => Stats.GetStatValue("WallClimbVelocityMultiplier");
	}
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
		_raycastBottomLeft = Storage.GetNode<RayCast2D>("RayCastBottomLeft");
		_raycastBottomRight = Storage.GetNode<RayCast2D>("RayCastBottomRight");
		_raycastTopLeft = Storage.GetNode<RayCast2D>("RayCastTopLeft");
		_raycastTopRight = Storage.GetNode<RayCast2D>("RayCastTopRight");
	}
	private bool IsTouchingLeftWall() => _raycastBottomLeft.IsColliding() || _raycastTopLeft.IsColliding();
	private bool IsTouchingRightWall() => _raycastBottomRight.IsColliding() || _raycastTopRight.IsColliding();
	private bool IsTouchingWall() => IsTouchingLeftWall() || IsTouchingRightWall();
	protected override void Enter()
	{
		if (Input.IsActionPressed("Climb"))
			_sprite.Play("WallClimb");
		else
			_sprite.Play("WallSlide");
		ResetJumps();
	}
	protected override void PhysicsUpdate(double delta)
	{
		if (Input.IsActionJustPressed("Climb"))
			_sprite.Play("WallClimb");
		if (Input.IsActionJustReleased("Climb"))
			_sprite.Play("WallSlide");

		Vector2 velocity = _player.Velocity;
		if (_sprite.Animation == "WallSlide")
			velocity.Y = _player.GetGravity().Y * WallSlideVelocityMultiplier;
		else
			velocity.Y = -_player.GetGravity().Y * WallClimbVelocityMultiplier;

		if ((Input.IsActionJustPressed("Left") && !HeadingLeft) ||
			(Input.IsActionJustPressed("Right") && HeadingLeft) ||
			_player.IsOnFloor() ||
			!IsTouchingWall())
			AskTransit("Idle");

		if (Input.IsActionJustPressed("Jump"))
		{
			int direction = HeadingLeft ? -1 : 1;
			velocity.X = -direction * Stats.GetStatValue("Speed");
			velocity.Y = -Stats.GetStatValue("JumpVelocity");
			HeadingLeft = !HeadingLeft;
			CanCoyoteTimerStart = false;
			AvailableJumps--;
			AskTransit("Idle");
		}

		if (Input.IsActionJustPressed("Dash") && AvailableDashes > 0)
		{
			HeadingLeft = !HeadingLeft;
			AskTransit("Dash");
		}
		_player.Velocity = velocity;
		_player.MoveAndSlide();
	}
	private void ResetJumps() => AvailableJumps = (int)Stats.GetStatValue("MaxJump");
}
