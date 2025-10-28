using Godot;
using System;

public partial class GuardianOfTheForest_NormalState : State
{
	[Export] public Area2D MeleeArea = null;
	private AnimatedSprite2D _sprite = null;
	private EnemyBase _enemy = null;
	private Player _player = null;
	private float MinDistanceToPerformMeleeAttack = 15.0f;
	private Vector2 MeleeAreaOffset = Vector2.Zero;
	private int HeadingRight
	{
		get => (int)Stats.GetStatValue("HeadingRight");
		set => Stats.SetValue("HeadingRight", value);
	}
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
		MeleeAreaOffset = MeleeArea.Position;
    }
	protected override void Enter()
	{
		_sprite.Play("Normal");
	}
	protected override void FrameUpdate(double delta)
    {
		HeadingRight = _player.GlobalPosition.X >= _enemy.GlobalPosition.X ? 1 : 0;

		if (_enemy.GlobalPosition.DistanceTo(ChasePos) <= MinDistanceToPerformMeleeAttack)
		{
			AskTransit("Melee");
			_enemy.Velocity *= 0.3f;
		}
    }
	protected override void PhysicsUpdate(double delta)
	{
		float dist = _enemy.GlobalPosition.DistanceTo(ChasePos);
		dist = Mathf.Clamp(dist, 100, 300);
		float speedFactor = dist / 100.0f;
		_enemy.Velocity = (ChasePos - _enemy.GlobalPosition).Normalized() * Stats.GetStatValue("Speed") * speedFactor;
		_enemy.MoveAndSlide();
	}
}
