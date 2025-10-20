using Godot;
using System;

public partial class Player_CanWallClimbState : State
{
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	private RayCast2D _raycastBottomLeft = null;
	private RayCast2D _raycastBottomRight = null;
	private RayCast2D _raycastTopLeft = null;
	private RayCast2D _raycastTopRight = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
		_raycastBottomLeft = Storage.GetNode<RayCast2D>("RayCastBottomLeft");
		_raycastBottomRight = Storage.GetNode<RayCast2D>("RayCastBottomRight");
		_raycastTopLeft = Storage.GetNode<RayCast2D>("RayCastTopLeft");
		_raycastTopRight = Storage.GetNode<RayCast2D>("RayCastTopRight");
	}
	private bool IsTouchingLeftWall() =>  _raycastBottomLeft.IsColliding() || _raycastTopLeft.IsColliding();
	private bool IsTouchingRightWall() => _raycastBottomRight.IsColliding() || _raycastTopRight.IsColliding();
	private bool IsTouchingWall() => IsTouchingLeftWall() || IsTouchingRightWall();
	protected override void PhysicsUpdate(double delta)
	{
		if (Input.IsActionPressed("Climb") && Stats.GetStatValue("CanWallClimb") >= 1f &&
				(
					(Input.IsActionPressed("Left") && Storage.GetVariant<bool>("HeadingLeft") && IsTouchingLeftWall()) ||
					(Input.IsActionPressed("Right") && !Storage.GetVariant<bool>("HeadingLeft") && IsTouchingRightWall())
				)
			)
			AskTransit("WallClimb");
	}
}
