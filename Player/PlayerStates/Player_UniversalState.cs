using Godot;
using System;

public partial class Player_UniversalState : State
{
	[Export] private Timer _invincibilityTimer = null;
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	private bool _isInvincible = false;
	private Tween _invincibilityTween = null;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
		_invincibilityTimer.Timeout += () =>
		{
			_isInvincible = false;
			_player.Modulate = Colors.White;
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
		for (int collisionIndex = 0; collisionIndex < _player.GetSlideCollisionCount(); collisionIndex++)
		{
			KinematicCollision2D collision = _player.GetSlideCollision(collisionIndex);
			if (!_isInvincible && collision.GetCollider() is ForestSpikeLayer)
			{
				HandleDamage();
				Flash();
				SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerHit);
				_isInvincible = true;
				_invincibilityTimer.Start();
			}
		}
	}
	public void HandleDamage()
	{
		ref int health = ref GameData.Instance.PlayerHealth;
		ref int shield = ref GameData.Instance.PlayerShield;

		if (shield > 0)
			shield--;
		else
		{
			health--;
			if (health == 0)
				SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerDied);
		}
	}
	public async void Flash()
	{
		Color pureWhite = new Color(18.892f, 18.892f, 18.892f, 1);
		Color originalModulate = _player.Modulate;
		_invincibilityTween = _player.CreateTween();
		_invincibilityTween
			.TweenProperty(_player, "modulate", pureWhite, 0.1f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.In);
		_invincibilityTween
			.TweenProperty(_player, "modulate", originalModulate, 0.1f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		await ToSignal(_invincibilityTween, Tween.SignalName.Finished);
		_invincibilityTween = _player.CreateTween();
		_invincibilityTween.SetLoops(10);
		_invincibilityTween.TweenProperty(_player, "modulate:a", 0.5f, 0.1f);
		_invincibilityTween.TweenProperty(_player, "modulate:a", 1, 0.1f);
	}
}
