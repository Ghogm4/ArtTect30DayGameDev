using Godot;
using System;
using System.Threading.Tasks;

public partial class Archer_MoveControlState : State
{
	private EnemyBase _enemy = null;
	private Player _player = null;
	private AnimatedSprite2D _sprite = null;
	private StatWrapper _speed = null;
	[Export] public float Acceleration = 10000f;
	[Export] public float Deceleration = 1000f;
	[Export] public float _maxFallSpeed = 500f;
	[Export] public float AttackCD = 3f;
	[Export] public float AttackRange = 200f;
	[Export] public float RollRange = 100f;
	[Export] public float RollCD = 2.5f;
	[Export] public float RollSpeed = 500f;
	private float _rollCooldownTimer = 0f;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		Storage.RegisterVariant<bool>("IsRunning", false);
		Storage.RegisterVariant<bool>("IsAttacking", false);
		Storage.RegisterVariant<bool>("IsAttackIdling", false);
		Storage.RegisterVariant<bool>("IsRolling", false);
		Storage.RegisterVariant<bool>("HasSpawnedArrow", false);
		_speed = new(Stats.GetStat("Speed"));
	}
	protected override void Enter()
	{
		GD.Print("Enter Archer MoveControl State");
	}
	protected override async void PhysicsUpdate(double delta)
	{
		if (_enemy.IsDead) AskTransit("Die");
		if (_rollCooldownTimer > 0)
		{
			_rollCooldownTimer -= (float)delta;
		}
		Vector2 velocity = _enemy.Velocity;
		if (!_enemy.IsOnFloor())
			velocity.Y = Math.Min(_enemy.GetGravity().Y * (float)delta * 0.5f + velocity.Y, _maxFallSpeed);

		if (Storage.GetVariant<bool>("IsRunning"))
		{
			// Accelerate towards player
			float direction = Math.Sign(_player.GlobalPosition.X - _enemy.GlobalPosition.X);
			velocity.X += direction * Acceleration * (float)delta;
			velocity.X = Mathf.Clamp(velocity.X, -(int)_speed, (int)_speed);
		}
		
		if (Storage.GetVariant<bool>("IsAttacking"))
		{
			velocity.X = 0;
		}

		if (velocity.X < 0)
			Storage.SetVariant("HeadingLeft", true);
		else if (velocity.X > 0)
			Storage.SetVariant("HeadingLeft", false);
		else
		{
			if (Storage.GetVariant<bool>("IsAttackIdling"))
			{
				if (_player.GlobalPosition.X < _enemy.GlobalPosition.X)
				{
					await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
					Storage.SetVariant("HeadingLeft", true);
				}
				else
				{
					await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
					Storage.SetVariant("HeadingLeft", false);
				}
			}
		}
		if (Storage.GetVariant<bool>("IsRolling") == false &&
			_enemy.GlobalPosition.DistanceTo(_player.GlobalPosition) <= RollRange &&
			(Storage.GetVariant<bool>("IsAttacking") || Storage.GetVariant<bool>("IsAttackIdling")) &&
			_rollCooldownTimer <= 0)
		{
			GD.Print("Enter Roll State");
			Storage.SetVariant("IsRolling", true);
			_rollCooldownTimer = RollCD;
			AskTransit("Roll");
			if (_player.GlobalPosition.X < _enemy.GlobalPosition.X)
			{
				Storage.SetVariant("RollDirection", 1);
			}
			else
			{
				Storage.SetVariant("RollDirection", -1);
			}
			velocity.X = RollSpeed * Storage.GetVariant<int>("RollDirection");
		}
		if (Storage.GetVariant<bool>("IsRolling"))
		{
			velocity.X = RollSpeed * Storage.GetVariant<int>("RollDirection");
		}
		if (Storage.GetVariant<bool>("IsRolling") && _enemy.GlobalPosition.DistanceTo(_player.GlobalPosition) > AttackRange - 10f)
		{
			GD.Print("Exit Roll State");
			Storage.SetVariant("IsRolling", false);
			if (_enemy.GlobalPosition.X < _player.GlobalPosition.X)
			{
				Storage.SetVariant("HeadingLeft", false);
			}
			else
			{
				Storage.SetVariant("HeadingLeft", true);
			}
		}
		if (_enemy.GlobalPosition.DistanceTo(_player.GlobalPosition) <= AttackRange && !Storage.GetVariant<bool>("IsAttacking"))
		{
			Storage.SetVariant("IsAttacking", true);
		}
		
		if (_enemy.GlobalPosition.DistanceTo(_player.GlobalPosition) > AttackRange + 20f && Storage.GetVariant<bool>("IsAttackIdling"))
		{
			GD.Print("Player moved out of attack idle range, resuming run.");
			Storage.SetVariant("IsAttacking", false);
			Storage.SetVariant("IsAttackIdling", false);
			Storage.SetVariant("IsRunning", true);
		}
		_enemy.Velocity = velocity;
	}

	
}
