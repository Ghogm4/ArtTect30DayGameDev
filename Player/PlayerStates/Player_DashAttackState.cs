using Godot;
using System;

public partial class Player_DashAttackState : State
{
	private Player _player = null;
	private AnimationPlayer _animationPlayer = null;
	private float AttackSpeed => Stats.GetStatValue("AttackSpeed");
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_animationPlayer = Storage.GetNode<AnimationPlayer>("AnimationPlayer");
	}
	protected override void Enter()
	{
		_animationPlayer.AnimationFinished += OnAnimationFinished;
		_animationPlayer.Play("DashAttack", -1, AttackSpeed);
		_player.Velocity = Vector2.Zero;
	}
	protected override void Exit()
	{
		_animationPlayer.AnimationFinished -= OnAnimationFinished;
	}

	private void OnAnimationFinished(StringName animName) => AskTransit("Idle");

}
