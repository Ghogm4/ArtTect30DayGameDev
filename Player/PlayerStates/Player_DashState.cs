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
		get => Storage.GetVariant<int>("AvailableJumps");
		set => Storage.SetVariant("AvailableJumps", value);
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
		_sprite.Play("Dash");
		_dashTimer.Start();
		_dashTrail.Emitting = true;
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
			velocity.X = Storage.GetVariant<int>("Speed") * DashSpeedMultiplier * direction;
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
		if (Input.IsActionJustPressed("Jump"))
		{
			velocity.Y = -Storage.GetVariant<float>("JumpVelocity");
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
