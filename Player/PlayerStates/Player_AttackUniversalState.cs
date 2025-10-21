using Godot;
using System;

public partial class Player_AttackUniversalState : State
{
	private AnimatedSprite2D _sprite = null;
	private Area2D _attackArea = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_attackArea = Storage.GetNode<Area2D>("AttackArea");
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
		foreach (var body in _attackArea.GetOverlappingBodies())
			if (body is EnemyBase enemy)
			{
				StatComponent enemyStats = enemy.GetNode<StatComponent>("StatComponent");
				PlayerStatComponent playerStats = Stats as PlayerStatComponent;
				foreach (var attackAction in playerStats.AttackActions)
					attackAction?.Invoke(enemyStats, Stats as PlayerStatComponent);
			}
	}
}
