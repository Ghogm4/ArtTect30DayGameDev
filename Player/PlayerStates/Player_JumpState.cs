using Godot;
using System;

public partial class Player_JumpState : State
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
		_sprite.Play("Rise");
		_sprite.AnimationFinished += OnAnimationFinished;

		if (previousState.Name == "Run")
			AudioManager.Instance.StopSFX("Run");
		AudioManager.Instance.PlaySFX("Jump");
	}
	protected override void FrameUpdate(double delta)
	{
		if (_player.IsOnFloor())
			AskTransit("Idle");
	}
	private void OnAnimationFinished()
	{
		_sprite.Play("Fall");
	}
	protected override void Exit()
	{
		_sprite.AnimationFinished -= OnAnimationFinished;
	}
}
