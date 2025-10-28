using Godot;
using System;

public partial class GuardianOfTheForest_MeleeState : State
{
	[Export] public Area2D MeleeArea = null;
	private EnemyBase _enemy = null;
	private AnimationPlayer _animationPlayer = null;
	private Player _player = null;
	private float _lastSpeed = 0.0f;
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
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_animationPlayer = Storage.GetNode<AnimationPlayer>("AnimationPlayer");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
	}
	protected override void Enter()
	{
		_animationPlayer.Play("Melee");
		_animationPlayer.AnimationFinished += OnAnimationFinished;
		_lastSpeed = _enemy.Velocity.Length() * 0.8f;
		Storage.SetVariant("CanTurnAround", false);
	}
	protected override void PhysicsUpdate(double delta)
	{
		_enemy.Velocity = (ChasePos - _enemy.GlobalPosition).Normalized() * _lastSpeed;
		_enemy.MoveAndSlide();
	}
	protected override void Exit()
	{
		_animationPlayer.AnimationFinished -= OnAnimationFinished;
		Storage.SetVariant("CanTurnAround", true);
	}
	private void OnAnimationFinished(StringName s) => AskTransit("Normal");
	private void DoMeleeAttack()
	{
		var bodies = MeleeArea.GetOverlappingBodies();
		foreach (var body in bodies)
			if (body is Player)
				_enemy.SendDamageRequest(Stats.GetStatValue("MeleeDamage"));
	}
	private void StandStill()
	{
		_lastSpeed = 0;
	}
}
