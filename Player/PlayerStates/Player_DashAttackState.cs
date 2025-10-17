using Godot;
using System;

public partial class Player_DashAttackState : State
{
	private const int FirstAttackFrame = 0;
	private const int SecondAttackFrame = 3;
	private bool _canAttack = true;
	private int _lastFrame = 0;
	private bool DealtDamageThisFrame = false;
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	private Area2D _attackArea = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
		_attackArea = Storage.GetNode<Area2D>("AttackArea");
	}
	protected override void Enter()
	{
		_sprite.AnimationFinished += OnAnimationFinished;
		_sprite.Play("DashAttack");
		_player.Velocity = Vector2.Zero;
	}
	protected override void FrameUpdate(double delta)
	{
		HandleAttack(delta);
	}
	private void HandleAttack(double delta)
	{
		if (_sprite.Frame == FirstAttackFrame || _sprite.Frame == SecondAttackFrame)
			_canAttack = true;
		else
			_canAttack = false;

		if (_lastFrame != _sprite.Frame)
			DealtDamageThisFrame = false;

		if (_canAttack && !DealtDamageThisFrame)
		{
			Attack();
			DealtDamageThisFrame = true;
		}
		_lastFrame = _sprite.Frame;
	}
	private void Attack()
	{
		GD.Print("Dash Attack!");
		foreach (var body in _attackArea.GetOverlappingBodies())
			if (body is EnemyBase enemy)
			{
				StatComponent enemyStats = enemy.GetNode<StatComponent>("StatComponent");
				PlayerStatComponent PlayerStats = Stats as PlayerStatComponent;
				foreach (var attackAction in PlayerStats.AttackActions)
					attackAction?.Invoke(enemyStats);
			}
	}
	protected override void Exit()
	{
		_sprite.AnimationFinished -= OnAnimationFinished;
	}

	private void OnAnimationFinished() => AskTransit("Idle");

}
