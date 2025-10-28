using Godot;
using System;

public partial class Player_AttackUniversalState : State
{
	private AnimatedSprite2D _sprite = null;
	private Area2D _attackArea = null;
	private Player _player = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_attackArea = Storage.GetNode<Area2D>("AttackArea");
		_player = Storage.GetNode<Player>("Player");
	}
	private void OnAnimationFinished()
	{
		AskTransit("Idle");
	}
	protected override void Enter()
	{
		_sprite.AnimationFinished += OnAnimationFinished;
	}
	protected override void PhysicsUpdate(double delta)
	{
		if (Input.IsActionJustPressed("Left") || Input.IsActionJustPressed("Right"))
			AskTransit("Idle");
		if (Input.IsActionJustPressed("Dash"))
			AskTransit("Dash");
	}

	protected override void Exit()
	{
		_sprite.AnimationFinished -= OnAnimationFinished;
	}
	public void Attack()
	{
		InvokeOnAttackActions();
		InvokeOnHittingEnemyActions();
	}
	private void InvokeOnAttackActions()
	{
		PlayerStatComponent playerStats = Stats as PlayerStatComponent;
		foreach (var onAttackAction in playerStats.OnAttackActions)
			onAttackAction?.Invoke(playerStats, _player.GlobalPosition);
	}
	private void InvokeOnHittingEnemyActions()
	{
		PlayerStatComponent playerStats = Stats as PlayerStatComponent;
		foreach (var body in _attackArea.GetOverlappingBodies())
			if (body is EnemyBase enemy)
				foreach (var onHittingEnemyAction in playerStats.OnHittingEnemyAction)
					onHittingEnemyAction?.Invoke(enemy, playerStats);
	}
}
