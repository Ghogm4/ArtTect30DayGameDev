using Godot;
using System;

public partial class Player_GravityAffectedState : State
{
	private Player _player = null;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
	}
	protected override void PhysicsUpdate(double delta)
	{
		Vector2 velocity = _player.Velocity;
		if (!_player.IsOnFloor())
			velocity += _player.GetGravity() * (float)delta * 0.5f;
		_player.Velocity = velocity;
    }
}
