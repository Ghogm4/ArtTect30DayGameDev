using Godot;
using System;

public partial class Player_Attack1State : State
{
	private AnimationPlayer _animationPlayer = null;
	private bool _canCombo = false;
	private float AttackSpeed => Stats.GetStatValue("AttackSpeed");
	protected override void ReadyBehavior()
	{
		_animationPlayer = Storage.GetNode<AnimationPlayer>("AnimationPlayer");
	}
	protected override void Enter()
	{
		_animationPlayer.Play("Attack1", -1, AttackSpeed);
		_animationPlayer.AnimationFinished += OnAnimationFinished;
		if (PreviousState != null && PreviousState.Name == "Run")
			AudioManager.Instance.StopSFX("Run");
		AudioManager.Instance.PlaySFX("Attack1");
	}
	protected override void FrameUpdate(double delta)
	{
		if (_canCombo && Input.IsActionJustPressed("Attack"))
			AskTransit("Attack2");
	}
	protected override void Exit()
	{
		_canCombo = false;
		_animationPlayer.Pause();
		_animationPlayer.AnimationFinished -= OnAnimationFinished;
	}
	private void EnableCombo() => _canCombo = true;
	private void OnAnimationFinished(StringName s) => AskTransit("Idle");
}
