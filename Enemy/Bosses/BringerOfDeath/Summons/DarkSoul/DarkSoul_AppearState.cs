using Godot;
using System;

public partial class DarkSoul_AppearState : State
{
	private AnimatedSprite2D _sprite;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
	}
	protected override void Enter()
	{
		_sprite.Play("Appear");
		_sprite.AnimationFinished += OnAnimationFinished;
	}
	protected override void Exit()
	{
		_sprite.AnimationFinished -= OnAnimationFinished;
	}
	private void OnAnimationFinished()
    {
        AskTransit("Float");
    }
}
