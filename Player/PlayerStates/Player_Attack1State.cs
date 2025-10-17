using Godot;
using System;

public partial class Player_Attack1State : State
{
	[Export] private Timer _attack1ComboTimer = null;
	private const int FirstAttackFrame = 2;
	private bool _canAttack = true;
	private int _lastFrame = 0;
	private bool DealtDamageThisFrame = false;
	private AnimatedSprite2D _sprite = null;
	private bool _canCombo = false;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_attack1ComboTimer.Timeout += OnAttack1ComboTimerTimeout;
	}
	protected override void Enter()
	{
		_sprite.Play("Attack1");
		_attack1ComboTimer.Start();

		if (PreviousState != null && PreviousState.Name == "Run")
			AudioManager.Instance.StopSFX("Run");
		AudioManager.Instance.PlaySFX("Attack1");
	}
	protected override void FrameUpdate(double delta)
	{
		if (_canCombo && Input.IsActionJustPressed("Attack"))
			AskTransit("Attack2");
		HandleAttack(delta);
	}
	protected override void Exit()
	{
		_canCombo = false;
		_attack1ComboTimer.Stop();
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
	private void OnAttack1ComboTimerTimeout() => _canCombo = true;
}
