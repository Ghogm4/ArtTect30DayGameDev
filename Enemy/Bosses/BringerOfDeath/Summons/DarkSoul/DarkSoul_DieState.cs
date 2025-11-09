using Godot;
using System;

public partial class DarkSoul_DieState : State
{
	[Export] public PackedScene DarkEssenceScene = null;
	private EnemyBase _enemy;
	private AnimatedSprite2D _sprite;
	private CollisionShape2D _hurtbox;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_hurtbox = Storage.GetNode<CollisionShape2D>("Hurtbox");
	}
	protected override void Enter()
	{
		_enemy.Velocity = Vector2.Zero;
		Storage.SetVariant("CanTurnAround", false);
		_sprite.Play("Die");
		CallDeferred(MethodName.DisableHurtbox);
		SpawnDarkEssence();
		_sprite.AnimationFinished += Disappear;
	}
	private void DisableHurtbox() => _hurtbox.Disabled = true;
	private void Disappear()
	{
		Tween tween = _enemy.CreateTween();
		tween.TweenProperty(_enemy, "modulate:a", 0f, 0.3f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.TweenCallback(Callable.From(_enemy.QueueFree));
	}
	private void SpawnDarkEssence()
	{
		DarkEssence darkEssence = Projectile.Factory.CreateProjectile<DarkEssence>(DarkEssenceScene);
		darkEssence.GlobalPosition = _enemy.GlobalPosition;
		darkEssence.Velocity = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau)) * GD.RandRange(100, 200);
		GetTree().CurrentScene.AddChild(darkEssence);
	}
}
