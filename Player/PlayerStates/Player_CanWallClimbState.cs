using Godot;
using System;

public partial class Player_CanWallClimbState : State
{
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
	}
	protected override void PhysicsUpdate(double delta)
	{
		if (_player.IsOnWallOnly() &&
				(
					(Input.IsActionPressed("Left") && Storage.GetVariant<bool>("HeadingLeft")) ||
					(Input.IsActionPressed("Right") && !Storage.GetVariant<bool>("HeadingLeft"))
				)
			)
			AskTransit("WallClimb");
	}
}
