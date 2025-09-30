using Godot;
using System;

public partial class Player_MoveControlState : State
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = 400.0f;
	private Player _player = null;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
	}
	protected override void PhysicsUpdate(double delta)
	{
		Vector2 velocity = _player.Velocity;
		bool isOnFloor = _player.IsOnFloor();

		if (!isOnFloor)
			velocity += _player.GetGravity() * (float)delta;

		if (Input.IsActionJustPressed("Jump") && isOnFloor)
			velocity.Y = -JumpVelocity;

		int direction = 0;
		if (Input.IsActionPressed("Right"))
			direction++;
		if (Input.IsActionPressed("Left"))
			direction--;

		if (direction != 0)
			velocity.X = direction * Speed;
		else
			velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);

		_player.Velocity = velocity;
		_player.MoveAndSlide();
	}
}
