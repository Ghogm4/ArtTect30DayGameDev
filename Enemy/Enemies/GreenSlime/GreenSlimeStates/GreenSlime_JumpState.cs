using Godot;
using System;

public partial class GreenSlime_JumpState : State
{
	private CharacterBody2D _enemy = null;
	private Player _player = null;
	private AnimatedSprite2D _sprite = null;
	
	[Signal] public delegate void JumpEventHandler();

	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<CharacterBody2D>("Enemy");
		_player = GetTree().GetRoot().FindChild("Player", true, false) as Player;
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		
		
	}
	protected override void Enter()
	{
		GD.Print("GreenSlime is now jumping.");
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
		await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
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
	
}
