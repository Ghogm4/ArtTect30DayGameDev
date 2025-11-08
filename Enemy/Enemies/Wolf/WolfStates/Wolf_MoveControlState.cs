using Godot;
using System;

public partial class Wolf_MoveControlState : State
{
	private EnemyBase _enemy = null;
	private Player _player = null;
	private AnimatedSprite2D _sprite = null;
	private StatWrapper _speed = null;
	[Export] public float Acceleration = 500f;
	[Export] public float Deceleration = 200f;
	[Export] public float _maxFallSpeed = 1000f;
	[Export] public float AttackRange = 50f;
	[Export] public float JumpRange = 60f;
	[Export] public float AttackCD = 1f;
	[Export] public float JumpCD = 3f;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		Storage.RegisterVariant<bool>("IsRunning", false);
		Storage.RegisterVariant<bool>("IsAttacking", false);
		Storage.RegisterVariant<bool>("IsAttackIdling", false);
		Storage.RegisterVariant<bool>("IsCharging", false);
		Storage.RegisterVariant<bool>("IsJumping", false);
		Storage.RegisterVariant<float>("JumpCooldown", 3f);
		_speed = new(Stats.GetStat("Speed"));
	}
	protected override void Enter()
	{
		GD.Print("Enter Wolf MoveControl State");
	}
	protected override async void PhysicsUpdate(double delta)
	{
		if (_enemy.IsDead) AskTransit("Die");

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
					await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
					Storage.SetVariant("HeadingLeft", true);
				}
				else
				{
					await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
					Storage.SetVariant("HeadingLeft", false);
				}
			}
		}

		if (_enemy.GlobalPosition.DistanceTo(_player.GlobalPosition) <= AttackRange && !Storage.GetVariant<bool>("IsAttacking"))
		{
			Storage.SetVariant("IsAttacking", true);
		}
		if (_enemy.GlobalPosition.DistanceTo(_player.GlobalPosition) > JumpRange && Storage.GetVariant<float>("JumpCooldown") <= 0f && !Storage.GetVariant<bool>("IsJumping") && !Storage.GetVariant<bool>("IsCharging"))
		{
			GD.Print("Triggering Charge for Jump");
    		Storage.SetVariant("JumpCooldown", JumpCD);
			AskTransit("Charge");
			Storage.SetVariant("IsAttacking", false);
			Storage.SetVariant("IsAttackIdling", false);
			Storage.SetVariant("IsRunning", false);
			Storage.SetVariant("IsCharging", true);
		}
		if (_enemy.GlobalPosition.DistanceTo(_player.GlobalPosition) > AttackRange + 20f && Storage.GetVariant<bool>("IsAttackIdling"))
		{
			GD.Print("Player moved out of attack idle range, resuming run.");
			Storage.SetVariant("IsAttacking", false);
			Storage.SetVariant("IsAttackIdling", false);
			Storage.SetVariant("IsRunning", true);
		}

		

		_enemy.Velocity = velocity;

		Storage.SetVariant("JumpCooldown", Storage.GetVariant<float>("JumpCooldown") - (float)delta);
		GD.Print(Storage.GetVariant<float>("JumpCooldown"));
	}	
}
