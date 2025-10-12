using Godot;
using System;

public partial class Player_GravityAffectedState : State
{
	private Player _player = null;
	[Export] private float _maxFallSpeed = 500f;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
	}
	protected override void PhysicsUpdate(double delta)
	{
		Vector2 velocity = _player.Velocity;
		if (!_player.IsOnFloor())
			velocity.Y = Math.Min(_player.GetGravity().Y * (float)delta * 0.5f + velocity.Y, _maxFallSpeed);
		_player.Velocity = velocity;
	}
}
