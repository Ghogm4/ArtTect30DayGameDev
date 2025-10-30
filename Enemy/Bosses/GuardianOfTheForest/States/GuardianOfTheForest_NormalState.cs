using Godot;
using System;

public partial class GuardianOfTheForest_NormalState : State
{
	[Export] public float WanderRadius = 50f;
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	private Player _player = null;
	private int HeadingRight => (int)Stats.GetStatValue("HeadingRight");
	private float Health => Stats.GetStatValue("Health");
	private float MaxHealth => Stats.GetStatValue("MaxHealth");
	private float Ratio => Mathf.Clamp(Health / MaxHealth, 0, 1);
	private float DurationFactor => Mathf.Lerp(1f, 0.9f, Ratio);
	private Vector2 _wanderPos = Vector2.Zero;
	private Vector2 GetRandomPosition(Vector2 pivot)
	{
		Vector2 offset = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau)) * (float)GD.RandRange(0, WanderRadius);
		return pivot + offset;
	}
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
	}
	protected override void Enter()
	{
		_sprite.Play("Normal");
		_wanderPos = GetRandomPosition(_player.GlobalPosition);
		GetTree().CreateTimer((float)GD.RandRange(3f, 4f) * DurationFactor).Timeout += () =>
		{
			if (!_enemy.IsDead)
				AskTransit("Decision");
		};
	}
	protected override void PhysicsUpdate(double delta)
	{
		if (_enemy.GlobalPosition.DistanceTo(_player.GlobalPosition) > WanderRadius)
			_wanderPos = _player.GlobalPosition;
		else if (_enemy.GlobalPosition.DistanceTo(_wanderPos) < 5f)
			_wanderPos = GetRandomPosition(_player.GlobalPosition) + Vector2.Up * WanderRadius / 2f;
		
		float distBoost = _enemy.GlobalPosition.DistanceTo(_wanderPos);
		distBoost = Mathf.Clamp(distBoost, 100, 300);
		float speedFactor = distBoost / 100.0f;
		_enemy.Velocity = (_wanderPos - _enemy.GlobalPosition).Normalized() * Stats.GetStatValue("Speed") * speedFactor;
		_enemy.MoveAndSlide();
	}
}
