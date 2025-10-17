using Godot;
using System;

public partial class Player_Attack2State : State
{
	[Export] private Timer _attack2ComboTimer = null;
	private const int FirstAttackFrame = 3;
	private bool _canAttack = true;
	private int _lastFrame = 0;
	private bool DealtDamageThisFrame = false;
	private AnimatedSprite2D _sprite = null;
	private Area2D _attackArea = null;
	private bool _canCombo = false;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_attackArea = Storage.GetNode<Area2D>("AttackArea");
		_attack2ComboTimer.Timeout += OnAttack2ComboTimerTimeout;
	}
	protected override void Enter()
	{
		_sprite.Play("Attack2");
		_attack2ComboTimer.Start();

		AudioManager.Instance.PlaySFX("Attack2");
	}
	protected override void FrameUpdate(double delta)
	{
		if (_canCombo && Input.IsActionJustPressed("Attack"))
			AskTransit("Attack3");
		HandleAttack(delta);
	}
	protected override void Exit()
	{
		_canCombo = false;
		_attack2ComboTimer.Stop();
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
	private void OnAttack2ComboTimerTimeout() => _canCombo = true;
}
