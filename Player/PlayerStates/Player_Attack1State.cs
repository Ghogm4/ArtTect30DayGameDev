using Godot;
using System;

public partial class Player_Attack1State : State
{
	[Export] private Timer _attack1ComboTimer = null;
	private AnimationPlayer _animationPlayer = null;
	private bool _canCombo = false;
	private float AttackSpeed => Stats.GetStatValue("AttackSpeed");
	protected override void ReadyBehavior()
	{
		_animationPlayer = Storage.GetNode<AnimationPlayer>("AnimationPlayer");
		_attack1ComboTimer.WaitTime /= AttackSpeed;
		_attack1ComboTimer.Timeout += OnAttack1ComboTimerTimeout;
	}
	protected override void Enter()
	{
		_animationPlayer.Play("Attack1", -1, AttackSpeed);
		_attack1ComboTimer.Start();

		if (PreviousState != null && PreviousState.Name == "Run")
			AudioManager.Instance.StopSFX("Run");
		AudioManager.Instance.PlaySFX("Attack1");
	}
	protected override void FrameUpdate(double delta)
	{
		if (_canCombo && Input.IsActionJustPressed("Attack"))
			AskTransit("Attack2");
	}
	protected override void Exit()
	{
		_canCombo = false;
		_attack1ComboTimer.Stop();
	}
	private void OnAttack1ComboTimerTimeout() => _canCombo = true;
}
