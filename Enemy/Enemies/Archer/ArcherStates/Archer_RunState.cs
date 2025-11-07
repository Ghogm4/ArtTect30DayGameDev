using Godot;
using System;
using System.Threading.Tasks;

public partial class Archer_RunState : State
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

	protected override async void Enter()
	{
		
		Storage.SetVariant("IsRunning", true);
		GD.Print("Enter Archer Run State");
		await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
		if (Storage.GetVariant<bool>("IsRunning") == false)
		{
			return;
		}
		_sprite.Play("Run");
		
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
