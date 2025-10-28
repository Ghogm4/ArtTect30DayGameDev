using Godot;
using System;

public partial class GuardianOfTheForest_NormalState : State
{
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	private Player _player = null;
	protected override void ReadyBehavior()
    {
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
    }
	protected override void Enter()
	{
		_sprite.Play("Normal");
	}
    protected override void PhysicsUpdate(double delta)
    {
		Vector2 velocity = _enemy.Velocity;
		velocity = (_player.GlobalPosition - _enemy.GlobalPosition).Normalized() * Stats.GetStatValue("Speed");
		_enemy.Velocity = velocity;
		_enemy.MoveAndSlide();
    }
}
