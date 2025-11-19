using Godot;
using System;

public partial class GreenSlime_JumpState : State
{
	private CharacterBody2D _enemy = null;
	private Player _player = null;
	private AnimatedSprite2D _sprite = null;

	private bool setCollide = false;
	[Signal] public delegate void JumpEventHandler();

	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<CharacterBody2D>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		
		
	}
	protected override void Enter()
	{
		
		Charge();
	}


	public void JumpUp()
	{
		_sprite.Stop();
		_sprite.Play("JumpUp");
		EmitSignal("Jump");
	}

	public async void Charge()
	{
		_sprite.Play("Charge");
		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
		JumpUp();
	}
	public void JumpDown()
	{
		_sprite.Play("JumpDown");
	}

	protected override void FrameUpdate(double delta)
	{
		if (!_enemy.IsOnFloor() && _enemy.Velocity.Y > 0 && _sprite.Animation != "JumpDown")
		{
			JumpDown();
		}

		if (_enemy.IsOnFloor() && _sprite.Animation == "JumpDown")
		{
			AskTransit("Jumpidle");
		}
	}

	protected override void PhysicsUpdate(double delta)
	{
		
		for (int i = 0; i < _enemy.GetSlideCollisionCount(); i++)
		{
			var collision = _enemy.GetSlideCollision(i);
			if (collision.GetCollider() == _player)
			{
				Storage.SetVariant("Is_Collision", true);
				if (!setCollide)
				{
					Storage.SetVariant("Colliding", true);
					setCollide = true;
				}
				break;
			}
			else setCollide = false;
		}
	}
	
}
