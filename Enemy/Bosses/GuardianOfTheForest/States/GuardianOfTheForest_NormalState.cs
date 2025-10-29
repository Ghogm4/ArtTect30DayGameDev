using Godot;
using System;

public partial class GuardianOfTheForest_NormalState : State
{
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	private Player _player = null;
	private int HeadingRight => (int)Stats.GetStatValue("HeadingRight");
	private float Health => Stats.GetStatValue("Health");
	private float MaxHealth => Stats.GetStatValue("MaxHealth");
	private float Ratio => Mathf.Clamp(Health / MaxHealth, 0, 1);
	private float DurationFactor => Mathf.Lerp(1f, 0.9f, Ratio);
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
	}
	protected override void Enter()
	{
		_sprite.Play("Normal");
		GetTree().CreateTimer((float)GD.RandRange(3f, 4f) * DurationFactor).Timeout += () =>
		{
			if (!_enemy.IsDead)
				AskTransit("Decision");
		};
	}
	protected override void PhysicsUpdate(double delta)
	{
		Vector2 playerPos = _player.GlobalPosition;
		float distBoost = _enemy.GlobalPosition.DistanceTo(playerPos);
		distBoost = Mathf.Clamp(distBoost, 100, 300);
		float speedFactor = distBoost / 100.0f;
		_enemy.Velocity = (playerPos - _enemy.GlobalPosition).Normalized() * Stats.GetStatValue("Speed") * speedFactor;
		_enemy.MoveAndSlide();
	}
}
