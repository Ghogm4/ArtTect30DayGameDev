using Godot;
using System;

public partial class GreenSlime_JumpidleState : State
{
	private AnimatedSprite2D _sprite = null;
	private CharacterBody2D _enemy = null;

	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<CharacterBody2D>("Enemy");
		_sprite.AnimationFinished += IdleDone;
	}

	protected override void Enter()
	{
		GD.Print("GreenSlime is now in Jumpidle state.");
		_sprite.Stop();
		_sprite.Play("Jumpidle");
	}

	public void IdleDone()
	{
		GD.Print("Idle animation finished.");
		_sprite.Stop();
		AskTransit("Jump");
		
	}
}
