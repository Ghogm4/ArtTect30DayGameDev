using Godot;
using System;

public partial class Skeleton_RunState : State
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
		Storage.SetVariant("IsRunning", true);
		_sprite.Stop();
		_sprite.Play("Run");
		GD.Print("Enter Skeleton Run State");
	}

	protected override void PhysicsUpdate(double delta)
	{
		if (Storage.GetVariant<bool>("IsAttacking"))
		{
			GD.Print("Transition to Attack State from Run State");
			AskTransit("Attack");
		}
	}
}
