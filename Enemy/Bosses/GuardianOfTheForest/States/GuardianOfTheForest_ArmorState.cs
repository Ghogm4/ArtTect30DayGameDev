using Godot;
using System;

public partial class GuardianOfTheForest_ArmorState : State
{
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
	}
	protected override void Enter()
	{
		_sprite.Play("Armor");
		_sprite.AnimationFinished += OnAnimationFinished;
		_enemy.Velocity = Vector2.Zero;
	}
	protected override void Exit()
	{
		_sprite.AnimationFinished -= OnAnimationFinished;
	}
	private void OnAnimationFinished()
	{
		Stats.AddFinal("DamageReduction", 0.1f);
		if (!_enemy.IsDead)
			AskTransit("Decision");
	}
}
