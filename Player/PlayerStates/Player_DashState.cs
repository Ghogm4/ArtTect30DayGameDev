using Godot;
using System;

public partial class Player_DashState : State
{
	[Export] public float DashSpeedMultiplier = 1f;
	[Export] private Timer _dashTimer = null;
	[Export] private GpuParticles2D _dashTrail = null;
	private Player _player = null;
	private AnimatedSprite2D _sprite = null;
	private int AvailableJumps
	{
		get => (int)Stats.GetStatValue("AvailableJumps");
		set => Stats.GetStat("AvailableJumps").AddFinal(value - (int)Stats.GetStatValue("AvailableJumps"));
	}
	private int AvailableDashes
	{
		get => (int)Stats.GetStatValue("AvailableDashes");
		set => Stats.GetStat("AvailableDashes").AddFinal(value - (int)Stats.GetStatValue("AvailableDashes"));
	}
	private bool CanCoyoteTimerStart
	{
		set => Storage.SetVariant("CanCoyoteTimerStart", value);
	}
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
	}
	protected override void Enter()
	{
		_player.EmitSignal(Player.SignalName.PlayerDashed);
		_sprite.Play("Dash");

		_dashTimer.WaitTime = Stats.GetStatValue("DashDuration");
		_dashTimer.Start();

		_dashTrail.Emitting = true;
		AvailableDashes--;
		CreateDashInvincibility();
		CreateDashCooldownTimer();

		HandleTrailDirection();
	}
	private void CreateDashInvincibility()
	{
		Storage.SetVariant("DashInvincible", true);
		Scheduler.Instance.ScheduleAction(Stats.GetStatValue("DashInvincibilityDuration"), () =>
		{
			Storage.SetVariant("DashInvincible", false);
		}, 0, true);
	}
	private void CreateDashCooldownTimer()
	{
		Scheduler.Instance.ScheduleAction(Stats.GetStatValue("DashCooldown"), () =>
		{
			if (AvailableDashes < (int)Stats.GetStatValue("MaxDash"))
				AvailableDashes++;
		}
		, 10, true);
	}
	private void HandleTrailDirection()
	{
		if (Storage.GetVariant<bool>("HeadingLeft"))
			_dashTrail.Texture = ResourceLoader.Load<Texture2D>("res://Assets/PlayerSpriteSheet/PlayerDashLeft.png");
		else
			_dashTrail.Texture = ResourceLoader.Load<Texture2D>("res://Assets/PlayerSpriteSheet/PlayerDashRight.png");
	}
	protected override void PhysicsUpdate(double delta)
	{
		int direction = Storage.GetVariant<bool>("HeadingLeft") ? -1 : 1;
		float tolerance = 1f;
		Vector2 velocity = _player.Velocity;
		if (!_dashTimer.IsStopped())
			velocity.X = Stats.GetStatValue("Speed") * DashSpeedMultiplier * direction;
		else
			velocity.X = Mathf.Lerp(velocity.X, 0, 0.7f);

		if (Input.IsActionJustPressed("Attack"))
			AskTransit("DashAttack");
		if (Mathf.Abs(velocity.X) < tolerance ||
			Input.IsActionJustPressed("Left") ||
			Input.IsActionJustPressed("Right"))
		{
			AskTransit("Idle");
			CanCoyoteTimerStart = false;
		}
		if (Input.IsActionJustPressed("Jump") && AvailableJumps > 0)
		{
			velocity.Y = -Stats.GetStatValue("JumpVelocity");
			AvailableJumps--;
			AskTransit("Idle");
			CanCoyoteTimerStart = false;
		}
		_player.Velocity = velocity;
		_player.MoveAndSlide();
	}
	protected override void Exit()
	{
		_dashTrail.Emitting = false;
	}
}
