using Godot;
using System;

public partial class Player_Attack3State : State
{
	private AnimatedSprite2D _sprite = null;
	private const int FirstAttackFrame = 2;
	private bool _canAttack = true;
	private int _lastFrame = 0;
	private bool DealtDamageThisFrame = false;
	private Area2D _attackArea = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_attackArea = Storage.GetNode<Area2D>("AttackArea");
	}
	protected override void Enter()
	{
		_sprite.Play("Attack3");

		AudioManager.Instance.PlaySFX("Attack3");
	}
	protected override void FrameUpdate(double delta)
	{
		HandleAttack(delta);
	}
	private void HandleAttack(double delta)
	{
		if (_sprite.Frame != FirstAttackFrame)
			_canAttack = false;
		else
			_canAttack = true;

		if (_lastFrame != _sprite.Frame)
			DealtDamageThisFrame = false;

		if (_canAttack && !DealtDamageThisFrame)
		{
			(Parent as Player_AttackUniversalState).Attack();
			DealtDamageThisFrame = true;
		}
		_lastFrame = _sprite.Frame;
	}
}
