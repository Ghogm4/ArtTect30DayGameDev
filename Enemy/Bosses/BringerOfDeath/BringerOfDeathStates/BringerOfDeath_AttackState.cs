using Godot;
using System;

public partial class BringerOfDeath_AttackState : State
{
	[Export] public float MinContinueAttackChance = 0.1f;
	[Export] public float MaxContinueAttackChance = 0.5f;
	[Export] public int MaxExtraAttacks = 2;
	private float ContinueAttackChance
	{
		get
		{
			float maxHealth = Stats.GetStatValue("MaxHealth");
			float health = Stats.GetStatValue("Health");
			return Mathf.Lerp(MaxContinueAttackChance, MinContinueAttackChance, health / maxHealth);
		}
	}
	private bool CanTurnAround
	{
		get => Storage.GetVariant<bool>("CanTurnAround");
		set => Storage.SetVariant("CanTurnAround", value);
	}
	private float WalkSpeed => Stats.GetStatValue("WalkSpeed");
	private EnemyBase _enemy;
	private AnimationPlayer _animationPlayer;
	private Area2D _attackArea;
	private int _attackCounter = 0;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_animationPlayer = Storage.GetNode<AnimationPlayer>("AnimationPlayer");
		_attackArea = Storage.GetNode<Area2D>("AttackArea");
	}
	private void Dash()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(_enemy, "velocity", _enemy.Velocity.Normalized() * WalkSpeed * 1.5f, 0.1f)
			.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(_enemy, "velocity", Vector2.Zero, 0.2f)
			.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
	}
	protected override void Enter()
	{
		Dash();
		_animationPlayer.Play("Attack");
		CanTurnAround = false;
		_animationPlayer.AnimationFinished += OnAnimationFinished;
	}
	protected override void Exit()
	{
		CanTurnAround = true;
		_animationPlayer.AnimationFinished -= OnAnimationFinished;
		_attackCounter = 0;
	}
	private void OnAnimationFinished(StringName str)
	{
		bool continueAttack = GD.Randf() < ContinueAttackChance;
		if (!continueAttack || _attackCounter >= MaxExtraAttacks)
		{
			AskTransit("Decision");
		}
		else
		{
			_animationPlayer.Play("Attack");
			_attackCounter++;
		}
	}
	private void Attack()
	{
		foreach (var body in _attackArea.GetOverlappingBodies())
		{
			if (body is not Player player) return;
			int damage = (int)Stats.GetStatValue("MeleeDamage");
			player.TakeDamage(damage, Callable.From<Player>(_enemy.CustomBehaviour));
		}
	}
}
