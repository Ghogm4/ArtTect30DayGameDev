using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Skeleton_UniversalState : State
{
	private EnemyBase _enemy = null;
	private AnimatedSprite2D _sprite = null;
	private CollisionShape2D attackArea = null;
	private CollisionShape2D chaseArea = null;
	private Vector2 attackPosition;
	private Vector2 chasePosition;

	// Called when the node enters the scene tree for the first time.
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		Storage.RegisterVariant<bool>("HeadingLeft", false);
		attackArea = _enemy.AttackArea.GetNode<CollisionShape2D>("CollisionShape2D");
		chaseArea = _enemy.ChaseArea.GetNode<CollisionShape2D>("CollisionShape2D");
		attackPosition = attackArea.Position;
		chasePosition = chaseArea.Position;
	}

	protected override void FrameUpdate(double delta)
	{
		bool headingLeft = Storage.GetVariant<bool>("HeadingLeft");
		if (headingLeft)
        {
			_sprite.FlipH = true;
			FlipArea(true);
        }	

		else
		{
			_sprite.FlipH = false;
			FlipArea(false);
		}
	}
	
	public void FlipArea(bool faceleft = false)
    {
        if (faceleft)
        {
            attackArea.Position = new Vector2(-attackPosition.X, attackPosition.Y);
			chaseArea.Position = new Vector2(-chasePosition.X, chasePosition.Y);
		}
		else
		{
			attackArea.Position = new Vector2(attackPosition.X, attackPosition.Y);
			chaseArea.Position = new Vector2(chasePosition.X, chasePosition.Y);
        }
    }
}
