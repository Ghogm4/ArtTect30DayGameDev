using Godot;
using System;

public partial class Player_DashAttackState : State
{
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
	}
	protected override void Enter()
	{
		_sprite.AnimationFinished += OnAnimationFinished;
		_sprite.Play("DashAttack");
		_player.Velocity = Vector2.Zero;
    }

    protected override void Exit()
	{
		_sprite.AnimationFinished -= OnAnimationFinished;
	}

	private void OnAnimationFinished() => AskTransit("Idle");
}
