using Godot;
using System;

public partial class Player_JumpState : State
{
	private const int _leapFrame = 2;
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

		if (PreviousState.Name == "Run")
			AudioManager.Instance.StopSFX("Run");
		AudioManager.Instance.PlaySFX("Jump");
	}
	protected override void FrameUpdate(double delta)
	{
		// The reason of including 0 is that the number of available jumps is first consumed by MoveControlState,
		// causing bug when available jump is exactly 1 if using Storage.GetVariant<int>("AvailableJumps") > 0
		if (Input.IsActionJustPressed("Jump") && Storage.GetVariant<int>("AvailableJumps") >= 0)
		{
			if (_sprite.Animation == "Fall")
				_sprite.Play("Rise");
			_sprite.Frame = _leapFrame;
		}
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
