using Godot;
using System;

public partial class Player_UniversalState : State
{
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
	}
	protected override void FrameUpdate(double delta)
	{
		bool headingLeft = Storage.GetVariant<bool>("HeadingLeft");
		if (headingLeft)
			_sprite.FlipH = true;
		else
			_sprite.FlipH = false;
	}
    protected override void PhysicsUpdate(double delta)
    {
        for (int collisionIndex = 0; collisionIndex < _player.GetSlideCollisionCount(); collisionIndex++)
		{
			KinematicCollision2D collision = _player.GetSlideCollision(collisionIndex);
			if (collision.GetCollider() is ForestSpikeLayer)
			{
				SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerHit);
			}
		}
    }
}
