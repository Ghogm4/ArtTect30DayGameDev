using Godot;
using System;

public partial class BringerOfDeath_WalkState : State
{
	[Export] public float Duration = 2.5f;
	private float WalkSpeed => Stats.GetStatValue("WalkSpeed");
	private SceneTreeTimer _timer;
	private EnemyBase _enemy;
	private AnimatedSprite2D _sprite;
	private Area2D _attackArea;
	private Player _player;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_attackArea = Storage.GetNode<Area2D>("AttackArea");
		_player = Storage.GetNode<Player>("Player");
	}
	protected override void Enter()
	{
		_sprite.Play("Walk");
		_attackArea.BodyEntered += OnBodyEntered;
		_timer = GetTree().CreateTimer(Duration);
		_timer.Timeout += OnTimeout;
	}
    protected override void PhysicsUpdate(double delta)
	{
		foreach (var body in _attackArea.GetOverlappingBodies())
			OnBodyEntered(body);
		Vector2 velocity = _enemy.Velocity;
		if (Mathf.Abs(_player.GlobalPosition.X - _enemy.GlobalPosition.X) < 10f)
			velocity.X = 0;
		else if (_player.GlobalPosition.X > _enemy.GlobalPosition.X)
			velocity.X = WalkSpeed;
		else
			velocity.X = -WalkSpeed;
		_enemy.Velocity = velocity;
    }
	protected override void Exit()
	{
		_attackArea.BodyEntered -= OnBodyEntered;
		_timer.Timeout -= OnTimeout;
	}
	private void OnBodyEntered(Node2D body)
	{
		if (body is not Player) return;
		AskTransit("Attack");
	}
	private void OnTimeout()
    {
		AskTransit("Attack");
    }
}
