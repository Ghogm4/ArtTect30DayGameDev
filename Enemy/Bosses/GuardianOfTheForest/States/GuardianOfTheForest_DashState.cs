using Godot;
using System;

public partial class GuardianOfTheForest_DashState : State
{
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	private Player _player = null;
	private float MinDistanceToPerformMeleeAttack = 15.0f;
	private Vector2 MeleeAreaOffset => Storage.GetVariant<Vector2>("MeleeAreaOffset");
	private int HeadingRight => (int)Stats.GetStatValue("HeadingRight");
	private Vector2 ChasePos
    {
		get
        {
			int dir = Mathf.IsEqualApprox(HeadingRight, 1) ? 1 : -1;
			Vector2 meleeAreaOffset = MeleeAreaOffset * new Vector2(dir, 1);
			Vector2 chasePos = _player.GlobalPosition - meleeAreaOffset;
			return chasePos;
        }
    }
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
	protected override void FrameUpdate(double delta)
    {
		if (_enemy.GlobalPosition.DistanceTo(ChasePos) <= MinDistanceToPerformMeleeAttack && !_enemy.IsDead)
			AskTransit("LaserCast");
    }
	protected override void PhysicsUpdate(double delta)
	{
		float dist = _enemy.GlobalPosition.DistanceTo(ChasePos);
		dist = Mathf.Clamp(dist, 30, 9999);
		float speedFactor = dist / 8.0f;
		_enemy.Velocity = (ChasePos - _enemy.GlobalPosition).Normalized() * Stats.GetStatValue("Speed") * speedFactor;
		_enemy.MoveAndSlide();
	}
}
