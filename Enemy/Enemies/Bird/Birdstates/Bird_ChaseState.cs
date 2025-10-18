using Godot;
using System;

public partial class Bird_ChaseState : State
{
	private EnemyBase _enemy = null;
	private Player _player = null;
	private AnimatedSprite2D _sprite = null;
	[Signal] public delegate void ChaseEventHandler();

	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");

	}

	protected override void Enter()
	{
		State idleState = GetNode<State>("../Idle");
		if (PreviousState == idleState)
		{
			_sprite.Stop();
			GD.Print("Transitioned from Idle to Chase");
		}
		
		_sprite.Play("Chase");
		EmitSignal("Chase");
		GD.Print("Enter Chase State");
	}

	protected override void PhysicsUpdate(double delta)
	{
		
	}
}
