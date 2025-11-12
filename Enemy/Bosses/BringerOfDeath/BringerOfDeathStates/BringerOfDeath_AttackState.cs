using Godot;
using System;

public partial class BringerOfDeath_AttackState : State
{
	[ExportGroup("Extra Attack Settings")]
	[Export] public float MinContinueAttackChance = 0.1f;
	[Export] public float MaxContinueAttackChance = 0.5f;
	[Export] public int MaxExtraAttacks = 2;
	[ExportGroup("Hand Attack Settings")]
	[Export] public float TriggerWhenUnderRatio = 0.5f;
	[Export] public int MinHandCount = 3;
	[Export] public int MaxHandCount = 5;
	[Export] public float HandGap = 20f;
	[Export] public float HandScale = 1f;
	[Export] public float HandSpawnDelay = 0.1f;
	[Export] public PackedScene HandScene;
	private const float HandYOffset = -32f;
	private int HandCount => (int)Mathf.Lerp(MaxHandCount, MinHandCount, Ratio);
	private float ContinueAttackChance
	{
		get
		{
			float maxHealth = Stats.GetStatValue("MaxHealth");
			float health = Stats.GetStatValue("Health");
			return Mathf.Lerp(MaxContinueAttackChance, MinContinueAttackChance, health / maxHealth);
		}
	}
	private bool CanTurnAround
	{
		get => Storage.GetVariant<bool>("CanTurnAround");
		set => Storage.SetVariant("CanTurnAround", value);
	}
	private float WalkSpeed => Stats.GetStatValue("WalkSpeed");
	private float Ratio => Stats.GetStatValue("Health") / Stats.GetStatValue("MaxHealth");
	private EnemyBase _enemy;
	private AnimationPlayer _animationPlayer;
	private Area2D _attackArea;
	private int _attackCounter = 0;
	protected override void ReadyBehavior()
	{
		_enemy = Storage.GetNode<EnemyBase>("Enemy");
		_animationPlayer = Storage.GetNode<AnimationPlayer>("AnimationPlayer");
		_attackArea = Storage.GetNode<Area2D>("AttackArea");
	}
	private void Dash()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(_enemy, "velocity", _enemy.Velocity.Normalized() * WalkSpeed * 1.5f, 0.1f)
			.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(_enemy, "velocity", Vector2.Zero, 0.2f)
			.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
	}
	protected override void Enter()
	{
		Player player = Storage.GetNode<Player>("Player");
		if (Mathf.Abs(player.GlobalPosition.X - _enemy.GlobalPosition.X) > 60f)
			Dash();
		else
			_enemy.Velocity = Vector2.Zero;
		_animationPlayer.Play("Attack");
		CanTurnAround = false;
		_animationPlayer.AnimationFinished += OnAnimationFinished;
	}
	protected override void Exit()
	{
		CanTurnAround = true;
		_animationPlayer.AnimationFinished -= OnAnimationFinished;
		_attackCounter = 0;
	}
	private void OnAnimationFinished(StringName str)
	{
		bool continueAttack = GD.Randf() < ContinueAttackChance;
		if (!continueAttack || _attackCounter >= MaxExtraAttacks)
		{
			AskTransit("Decision");
		}
		else
		{
			_animationPlayer.Play("Attack");
			_attackCounter++;
		}
	}
	private void Attack()
	{
		foreach (var body in _attackArea.GetOverlappingBodies())
		{
			if (body is not Player player) return;
			int damage = (int)Stats.GetStatValue("MeleeDamage");
			player.TakeDamage(damage, Callable.From<Player>(_enemy.CustomBehaviour));
		}
		if (Ratio <= TriggerWhenUnderRatio)
		{
			CreateHands();
		}
	}
	private async void CreateHands()
	{
		int direction = Storage.GetVariant<bool>("HeadingLeft") ? -1 : 1;
		float currentXOffset = direction * HandGap;
        for (int i = 0; i < HandCount; i++)
		{
			float spread = Mathf.Pi / 10;
			float radian = (float)GD.RandRange(-Mathf.Pi - spread, -Mathf.Pi + spread);
			Vector2 spawnPosition = _enemy.GlobalPosition + Vector2.Right * currentXOffset;

			var handInstance = HandScene.Instantiate<Hand>();
			handInstance.GlobalPosition = spawnPosition + Vector2.Up * HandYOffset;
			handInstance.Rotation = radian;
			handInstance.Scale = Vector2.One * HandScale;
			GetTree().CurrentScene.CallDeferred(MethodName.AddChild, handInstance);

			currentXOffset += HandGap * direction;
			await ToSignal(GetTree().CreateTimer(HandSpawnDelay), SceneTreeTimer.SignalName.Timeout);
        }
    }
}
