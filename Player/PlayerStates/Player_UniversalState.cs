using Godot;
using System;

public partial class Player_UniversalState : State
{
	[Export] private Timer _invincibilityTimer = null;
	private AnimatedSprite2D _sprite = null;
	private Player _player = null;
	private Area2D _attackArea = null;
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
		_attackArea = Storage.GetNode<Area2D>("AttackArea");
		_invincibilityTimer.Timeout += () =>
		{
			_isInvincible = false;
			_sprite.Modulate = Colors.White;
			_invincibilityTween?.Kill();
		};
		InitializeSignals();
		InitializeWrappers();
		EmitHealthStatus(0, 0);
	}
	private void InitializeSignals()
	{
		Stats.GetStat("Health").StatChanged += EmitHealthStatus;
		Stats.GetStat("MaxHealth").StatChanged += EmitHealthStatus;
		Stats.GetStat("Shield").StatChanged += EmitHealthStatus;
		Stats.GetStat("Coin").StatChanged += EmitHealthStatus;
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
		bool headingLeft = Storage.GetVariant<bool>("HeadingLeft");
		if (headingLeft)
			_attackArea.Scale = -Vector2.One;
		else
			_attackArea.Scale = Vector2.One;

		if (_sprite.Animation == "Die")
		{
			_sprite.Modulate = Colors.White;
			_invincibilityTween?.Kill();
			return;
		}
		if (_isInvincible)
			return;
		for (int collisionIndex = 0; collisionIndex < _player.GetSlideCollisionCount(); collisionIndex++)
		{
			KinematicCollision2D collision = _player.GetSlideCollision(collisionIndex);

			if (collision.GetCollider() is DamagingTileMapLayer damagingLayer)
			{
				SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerHit, damagingLayer.Damage,
					Callable.From<Player>((player) => { }));
			}
			else if (collision.GetCollider() is MoveHandler moveHandler && moveHandler.CanDoDamage)
			{
				SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerHit, moveHandler.Damage,
					Callable.From<Player>((player) => { }));
			}
		}
	}

	public void OnPlayerHit(int damage, Callable customBehavior)
	{
		int remainingDamage = damage;
		Callable behavior = customBehavior;
		float evasion = Mathf.Clamp(Stats.GetStatValue("Evasion"), 0f, 100f) / 100f;
		Probability.RunSingle(evasion, () =>
		{
			remainingDamage = 0;
			behavior = Callable.From<Player>((player) => { });
		});
		int shieldReceivedDamage = Mathf.Min((int)_shield, remainingDamage);
		_shield -= shieldReceivedDamage;
		remainingDamage -= shieldReceivedDamage;

		_health -= remainingDamage;
		behavior.Call(_player);
		EmitHealthStatus(0, 0);

		if (_health <= 0)
		{
			AskTransit("Die");
			return;
		}

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
	private void EmitHealthStatus(float oldValue, float newValue)
	{
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerHealthStatusUpdated, (int)_health, (int)_maxHealth, (int)_shield, (int)Stats.GetStatValue("Coin"));
	}
}
