using Godot;
using System;

public partial class BringerOfDeath_IdleState : State
{
	[Export] public float MaxDuration = 2.5f;
	[Export] public float MinDuration = 1.8f;
	private float Duration
    {
		get
        {
			float maxHealth = Stats.GetStatValue("MaxHealth");
			float health = Stats.GetStatValue("Health");
			return Mathf.Lerp(MaxDuration, MinDuration, health / maxHealth);
        }
    }
	private EnemyBase _enemy;
	private AnimatedSprite2D _sprite;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
	}
	protected override void Enter()
	{
		_sprite.Play("Idle");
		GetTree().CreateTimer(Duration).Timeout += () =>
		{
			if (IsInstanceValid(_enemy) && !_enemy.IsDead)
				AskTransit("Decision");
		};
	}
}
