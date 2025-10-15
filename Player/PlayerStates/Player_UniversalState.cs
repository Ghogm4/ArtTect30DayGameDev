using Godot;
using System;

public partial class Player_UniversalState : State
{
	[Export] private Timer _invincibilityTimer = null;
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	private bool _isInvincible = false;
	private Tween _invincibilityTween = null;
	private StatWrapper _health;
	private StatWrapper _shield;
	private StatWrapper _maxHealth;
	private StatWrapper _invincibilityTime;
	protected override void ReadyBehavior()
	{
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_player = Storage.GetNode<Player>("Player");
		_invincibilityTimer.Timeout += () =>
		{
			_isInvincible = false;
			_sprite.Modulate = Colors.White;
			_invincibilityTween?.Kill();
		};
		InitializeSignals();
		InitializeWrappers();
		EmitHealthStatus();
	}
	private void InitializeSignals()
	{
		Stats.GetStat("Health").StatChanged += EmitHealthStatus;
		Stats.GetStat("MaxHealth").StatChanged += EmitHealthStatus;
		Stats.GetStat("Shield").StatChanged += EmitHealthStatus;
		SignalBus.Instance.PlayerHit += OnPlayerHit;
		SignalBus.Instance.RegisterSceneChangeStartedAction(() => SignalBus.Instance.PlayerHit -= OnPlayerHit, SignalBus.Priority.Super);
	}
	private void InitializeWrappers()
	{
		_health = new(Stats.GetStat("Health"));
		_shield = new(Stats.GetStat("Shield"));
		_maxHealth = new(Stats.GetStat("MaxHealth"));
		_invincibilityTime = new(Stats.GetStat("InvincibilityTime"));
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
		if (_sprite.Animation == "Die")
		{
			_sprite.Modulate = Colors.White;
			_invincibilityTween?.Kill();
			return;
		}
		for (int collisionIndex = 0; collisionIndex < _player.GetSlideCollisionCount(); collisionIndex++)
		{
			KinematicCollision2D collision = _player.GetSlideCollision(collisionIndex);
			if (!_isInvincible && collision.GetCollider() is ForestSpikeLayer)
				SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerHit, 1,
				Callable.From<Player>((player) => {}));
		}
	}
	
	public void OnPlayerHit(int damage, Callable customBehavior)
	{
		int remainingDamage = damage;

		int shieldReceivedDamage = Mathf.Min((int)_shield, remainingDamage);
		_shield -= shieldReceivedDamage;
		remainingDamage -= shieldReceivedDamage;

		_health -= remainingDamage;
		customBehavior.Call(_player);
		EmitHealthStatus();
		
		if (_health <= 0)
		{
			AskTransit("Die");
			return;
		}
		
		EmitHealthStatus();
		SetInvincible();
	}
	private void SetInvincible()
	{
		Flash();
		_isInvincible = true;
		_invincibilityTimer.WaitTime = (float)_invincibilityTime;
		_invincibilityTimer.Start();
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
		float flashSequenceLength = 0.2f;
		int loops = (int)Mathf.Ceil((float)_invincibilityTime / flashSequenceLength);
		_invincibilityTween.SetLoops(loops);
		_invincibilityTween.TweenProperty(_sprite, "modulate:a", 0.5f, 0.1f);
		_invincibilityTween.TweenProperty(_sprite, "modulate:a", 1, 0.1f);
	}
	private void EmitHealthStatus()
	{
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerHealthStatusUpdated, (int)_health, (int)_maxHealth, (int)_shield);
	}
}