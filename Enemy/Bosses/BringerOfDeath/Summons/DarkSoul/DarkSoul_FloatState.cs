using Godot;
using System;

public partial class DarkSoul_FloatState : State
{
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
		_sprite.Play("Float");
		Storage.SetVariant("CanTurnAround", true);
		CallDeferred(MethodName.EnableHurtbox);
	}
	private void EnableHurtbox() => _hurtbox.Disabled = false;
}
