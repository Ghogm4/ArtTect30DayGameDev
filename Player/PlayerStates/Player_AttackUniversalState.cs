using Godot;
using System;

public partial class Player_AttackUniversalState : State
{
	private AnimatedSprite2D _sprite = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
	}
	private void OnAnimationFinished()
	{
		AskTransit("Idle");
	}
	protected override void Enter()
	{
		_sprite.AnimationFinished += OnAnimationFinished;
	}
    protected override void Exit()
    {
		_sprite.AnimationFinished -= OnAnimationFinished;
    }
}
