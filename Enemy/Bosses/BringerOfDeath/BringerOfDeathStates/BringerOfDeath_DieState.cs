using Godot;
using System;

public partial class BringerOfDeath_DieState : State
{
	private EnemyBase _enemy;
	private AnimatedSprite2D _sprite;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
	}
    protected override void Enter()
	{
		_enemy.Velocity = Vector2.Zero;
		_sprite.Play("Die");
		_enemy.Die();
    }
}
