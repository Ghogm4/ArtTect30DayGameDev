using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Wolf_UniversalState : State
{
	private EnemyBase _enemy = null;
	private AnimatedSprite2D _sprite = null;
    private CollisionShape2D attackArea = null;
    private AnimatedSprite2D _attackEffectSprite = null;
    private Vector2 attackEffectPosition;
	private Vector2 attackPosition;

	// Called when the node enters the scene tree for the first time.
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        
		Storage.RegisterVariant<bool>("HeadingLeft", false);
		attackArea = _enemy.AttackArea.GetNode<CollisionShape2D>("CollisionShape2D");
        attackPosition = attackArea.Position;

        _attackEffectSprite = _enemy.GetNode<AnimatedSprite2D>("AnimatedSprite2D2");
        attackEffectPosition = _attackEffectSprite.Position;
	}

	protected override void FrameUpdate(double delta)
	{
		bool headingLeft = Storage.GetVariant<bool>("HeadingLeft");
		if (headingLeft)
        {
            _sprite.FlipH = false;
            _attackEffectSprite.FlipH = true;
            FlipArea(false);
            FlipAttackEffectPosition(false);
        }	

		else
		{
            _sprite.FlipH = true;
            _attackEffectSprite.FlipH = false;
            FlipArea(true);
            FlipAttackEffectPosition(true);
		}
	}

    public void FlipArea(bool faceleft = false)
    {
        if (faceleft)
        {
            attackArea.Position = new Vector2(-attackPosition.X, attackPosition.Y);
        }
        else
        {
            attackArea.Position = new Vector2(attackPosition.X, attackPosition.Y);
        }
    }
    public void FlipAttackEffectPosition(bool faceleft = false)
    {
        if (faceleft)
        {
            _attackEffectSprite.Position = new Vector2(-attackEffectPosition.X, attackEffectPosition.Y);
        }
        else
        {
            _attackEffectSprite.Position = new Vector2(attackEffectPosition.X, attackEffectPosition.Y);
        }
    }
}
