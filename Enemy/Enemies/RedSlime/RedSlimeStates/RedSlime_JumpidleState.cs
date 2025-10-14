using Godot;
using System;

public partial class RedSlime_JumpidleState : State
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
		
		_sprite.Stop();
		_sprite.Play("Jumpidle");
		Storage.SetVariant("Is_Collision", false);
	}

	public void IdleDone()
	{
		
		_sprite.Stop();
		AskTransit("Jump");
		
	}
}
