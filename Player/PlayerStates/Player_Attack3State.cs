using Godot;
using System;

public partial class Player_Attack3State : State
{
	private AnimationPlayer _animationPlayer = null;
	private float AttackSpeed => Stats.GetStatValue("AttackSpeed");
	protected override void ReadyBehavior()
	{
		_animationPlayer = Storage.GetNode<AnimationPlayer>("AnimationPlayer");
	}
	protected override void Enter()
	{
		_animationPlayer.Play("Attack3", -1, AttackSpeed);
		AudioManager.Instance.PlaySFX("Attack3");
		_animationPlayer.AnimationFinished += OnAnimationFinished;
	}
	protected override void Exit()
	{
		_animationPlayer.Stop();
		_animationPlayer.AnimationFinished -= OnAnimationFinished;
	}
	private void OnAnimationFinished(StringName s) => AskTransit("Idle");
}
