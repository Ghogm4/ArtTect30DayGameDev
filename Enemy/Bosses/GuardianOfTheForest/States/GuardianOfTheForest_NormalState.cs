using Godot;
using System;

public partial class GuardianOfTheForest_NormalState : State
{
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	private Player _player = null;
	private RandomNumberGenerator _rng = new();
	private int HeadingRight => (int)Stats.GetStatValue("HeadingRight");
	protected override void ReadyBehavior()
    {
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
    }
	protected override void Enter()
	{
		_sprite.Play("Normal");
		_rng.Randomize();
		GetTree().CreateTimer(_rng.RandfRange(1f, 3f)).Timeout += () => AskTransit("Dash");
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
