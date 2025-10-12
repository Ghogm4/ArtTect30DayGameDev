using Godot;
using System;

public partial class Enemy_UniversalState : State
{
	[Export] private Timer _invincibilityTimer = null;
	private AnimatedSprite2D _sprite = null;
	private Enemy _enemy = null;
	private bool _isInvincible = false;
	private Tween _invincibilityTween = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemy = Storage.GetNode<Enemy>("Enemy");
		_invincibilityTimer.Timeout += () =>
		{
			_isInvincible = false;
			_sprite.Modulate = Colors.White;
			if (IsInstanceValid(_invincibilityTween))
				_invincibilityTween.Kill();
		};
	}
	protected override void FrameUpdate(double delta)
	{
		bool headingLeft = Storage.GetVariant<bool>("HeadingLeft");
		if (headingLeft)
			_sprite.FlipH = true;
		else
			_sprite.FlipH = false;
	}

	protected override void PhysicsUpdate(double delta)
	{
		for (int collisionIndex = 0; collisionIndex < _enemy.GetSlideCollisionCount(); collisionIndex++)
		{
			KinematicCollision2D collision = _enemy.GetSlideCollision(collisionIndex);
			if (!_isInvincible && collision.GetCollider() is ForestSpikeLayer)
			{
				HandleDamage();
				Flash();
				_isInvincible = true;
				_invincibilityTimer.Start();
			}
		}

		

	}
	public void HandleDamage()
	{
		_enemy.Health -= 10;
		if (_enemy.Health <= 0)
		{
			AskTransit("Die");
		}
	}
	
	public async void Flash()
	{
		Color pureWhite = new Color(18.892f, 18.892f, 18.892f, 1);
		Color originalModulate = _sprite.Modulate;
		_invincibilityTween = _sprite.CreateTween();
		_invincibilityTween
			.TweenProperty(_sprite, "modulate", pureWhite, 0.1f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.In);
		_invincibilityTween
			.TweenProperty(_sprite, "modulate", originalModulate, 0.1f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		await ToSignal(_invincibilityTween, Tween.SignalName.Finished);
		_invincibilityTween = _sprite.CreateTween();
		_invincibilityTween.SetLoops(10);
		_invincibilityTween.TweenProperty(_sprite, "modulate:a", 0.5f, 0.1f);
		_invincibilityTween.TweenProperty(_sprite, "modulate:a", 1, 0.1f);
	}

	public void Monitor()
	{

	}
	
	protected override void Enter()
	{
		GD.Print("Enemy Entered Universal State");
	}
}
