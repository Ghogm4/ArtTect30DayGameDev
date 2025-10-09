using Godot;
using System;

public partial class Player_DashState : State
{
	[Export] public float DashSpeedMultiplier = 1f;
	[Export] private Timer _dashTimer = null;
	[Export] private GpuParticles2D _dashTrail = null;
	private Player _player = null;
	private AnimatedSprite2D _sprite = null;
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
		_player.Velocity = velocity;
		_player.MoveAndSlide();
		if (Mathf.Abs(velocity.X) < tolerance ||
			Input.IsActionJustPressed("Left") ||
			Input.IsActionJustPressed("Right") ||
			Input.IsActionJustPressed("Jump"))
			AskTransit("Idle");
	}
	protected override void Exit()
	{
		_dashTrail.Emitting = false;
	}
}
