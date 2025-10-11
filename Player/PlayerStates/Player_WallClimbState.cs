using Godot;
using System;

public partial class Player_WallClimbState : State
{
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	private int AvailableJumps
	{
		get => Storage.GetVariant<int>("AvailableJumps");
		set => Storage.SetVariant("AvailableJumps", value);
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
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
	}
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
			velocity.Y = _player.GetGravity().Y * 0.05f;
		else
			velocity.Y = -_player.GetGravity().Y * 0.05f;

		if ((Input.IsActionJustPressed("Left") && !HeadingLeft) ||
			(Input.IsActionJustPressed("Right") && HeadingLeft) ||
			!_player.IsOnWallOnly())
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

		if (Input.IsActionJustPressed("Dash"))
		{
			HeadingLeft = !HeadingLeft;
			AskTransit("Dash");
		}
		_player.Velocity = velocity;
		_player.MoveAndSlide();
	}
	private void ResetJumps() => Storage.SetVariant("AvailableJumps", GameData.Instance.PlayerMaxJumps);
}
