using Godot;
using System;

public partial class Player_JumpState : State
{
	private const int LeapFrame = 2;
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	private bool _canTriggerRise = true;
	private int AvailableJumps
	{
		get => (int)Stats.GetStatValue("AvailableJumps");
		set => Stats.GetStat("AvailableJumps").AddFinal(value - (int)Stats.GetStatValue("AvailableJumps"));
	}
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

		_canTriggerRise = true;
	}
	protected override void FrameUpdate(double delta)
	{
		// The reason of including 0 is that the number of available jumps is first consumed by MoveControlState,
		// causing bug when available jump is exactly 1 if using Storage.GetVariant<int>("AvailableJumps") > 0
		if (Input.IsActionJustPressed("Jump") && _canTriggerRise)
		{
			if (_sprite.Animation == "Fall")
				_sprite.Play("Rise");
			_sprite.Frame = LeapFrame;
		}
		if (AvailableJumps <= 0)
			_canTriggerRise = false;
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
