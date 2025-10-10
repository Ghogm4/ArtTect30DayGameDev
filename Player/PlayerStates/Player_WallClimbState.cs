using Godot;
using System;

public partial class Player_WallClimbState : State
{
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	public int AvailableJumps => Storage.GetVariant<int>("AvailableJumps");
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

		if ((Input.IsActionJustPressed("Left") && !Storage.GetVariant<bool>("HeadingLeft")) ||
			(Input.IsActionJustPressed("Right") && Storage.GetVariant<bool>("HeadingLeft")) ||
			!_player.IsOnWallOnly())
			AskTransit("Idle");

		if (Input.IsActionJustPressed("Jump"))
		{
			int direction = Storage.GetVariant<bool>("HeadingLeft") ? -1 : 1;
			velocity.X = -direction * Storage.GetVariant<float>("Speed");
			velocity.Y = -Storage.GetVariant<float>("JumpVelocity");
			Storage.SetVariant("HeadingLeft", !Storage.GetVariant<bool>("HeadingLeft"));
			DecreaseJump();
			AskTransit("Idle");
		}

		if (Input.IsActionJustPressed("Dash"))
		{
			Storage.SetVariant("HeadingLeft", !Storage.GetVariant<bool>("HeadingLeft"));
			AskTransit("Dash");
		}
		_player.Velocity = velocity;
		_player.MoveAndSlide();
	}
	private void ResetJumps() => Storage.SetVariant("AvailableJumps", GameData.Instance.PlayerMaxJumps);
	private void DecreaseJump() => Storage.SetVariant("AvailableJumps", AvailableJumps - 1);
}
