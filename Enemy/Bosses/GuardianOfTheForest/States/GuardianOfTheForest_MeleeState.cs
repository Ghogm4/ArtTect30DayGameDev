using Godot;
using System;

public partial class GuardianOfTheForest_MeleeState : State
{
	[Export] public Area2D MeleeArea = null;
	private EnemyBase _enemy = null;
	private AnimationPlayer _animationPlayer = null;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_animationPlayer = Storage.GetNode<AnimationPlayer>("AnimationPlayer");
	}
	protected override void Enter()
	{
		_animationPlayer.Play("Melee");
		_animationPlayer.AnimationFinished += OnAnimationFinished;
	}
	protected override void Exit()
	{
		_animationPlayer.AnimationFinished -= OnAnimationFinished;
	}
	private void OnAnimationFinished(StringName s) => AskTransit("Normal");
	private void DoMeleeAttack()
	{
		var bodies = MeleeArea.GetOverlappingBodies();
		foreach (var body in bodies)
			if (body is Player player)
				_enemy.SendDamageRequest(Stats.GetStatValue("MeleeDamage"));
	}
}
