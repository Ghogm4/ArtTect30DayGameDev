using Godot;
using System;

public partial class Wolf_AttackState : State
{
    private AnimatedSprite2D _sprite = null;
    private AnimatedSprite2D _attackEffectSprite = null;
	private EnemyBase _enemy = null;
	private Player _player = null;
	private Vector2 _playerLastPosition = Vector2.Zero;

	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
        _attackEffectSprite = _enemy.GetNode<AnimatedSprite2D>("AnimatedSprite2D2");
		_sprite.AnimationFinished += () =>
		{
			_sprite.Stop();
			AskTransit("AttackIdle");
		};
        Storage.RegisterVariant<bool>("HasDealtDamage", false);
        _attackEffectSprite.Visible = false;
        _attackEffectSprite.AnimationFinished += () =>
        {
            _attackEffectSprite.Visible = false;
            _attackEffectSprite.Stop();
        };
	}

	

	protected override void Enter()
	{
		GD.Print("Enter Wolf Attack State");

		// 停止所有动作
		Storage.SetVariant("IsAttackIdling", false);
		Storage.SetVariant("IsAttacking", true);
		Storage.SetVariant("IsRunning", false);
		Storage.SetVariant("HasDealtDamage", false);
		_enemy.Velocity = new Vector2(0, _enemy.Velocity.Y);
		_sprite.Play("Attack");

		
	}

	protected override void FrameUpdate(double delta)
	{
        if (_sprite.Frame == 3 && Storage.GetVariant<bool>("HasDealtDamage") == false)
        {
            _attackEffectSprite.Visible = true;
            _attackEffectSprite.Play();
            DealDamage();
            Storage.SetVariant("HasDealtDamage", true);
        }

	}
	public void DealDamage()
	{
		GD.Print("Wolf Attack 1 Damage Dealt");
		foreach (Node body in _enemy.AttackArea.GetOverlappingBodies())
		{
			if (body is Player player)
			{
				GD.Print("Player Hit by Wolf Attack ");
				player.TakeDamage(1, Callable.From<Player>((player) => { }));
			}
		}
	}
}
