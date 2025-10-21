using Godot;
using System;
using System.Threading.Tasks;
public partial class EnemyBase : CharacterBody2D
{
	[ExportGroup("Coin Settings")]
	[Export] public int MinCoinDrop = 10;
	[Export] public int MaxCoinDrop = 50;
	[Export] public float CoinDropRate = 0.5f;
	[ExportGroup("Soul Settings")]
	[Export] public int MinSoulDrop = 5;
	[Export] public int MaxSoulDrop = 20;
	[Export] public float SoulDropRate = 0.3f;
	[ExportGroup("Components")]
	[Export] public Area2D MonitorArea;
	[Export] public Area2D AttackArea;
	[Export] public Area2D ChaseArea;
	[Export] public VarStorage Storage;
	[Export] public AnimatedSprite2D Sprite;
	[Export] public StatComponent Stats;
	[Export] public TextureProgressBar HealthBar;
	[Export] public PackedScene FloatingTextScene;

	[Signal] public delegate void EnterMonitorEventHandler(Node2D body);
	[Signal] public delegate void EnterChaseEventHandler(Node2D body);
	[Signal] public delegate void EnterAttackEventHandler(Node2D body);
	[Signal] public delegate void ExitAttackEventHandler(Node2D body);
	[Signal] public delegate void ExitChaseEventHandler(Node2D body);
	[Signal] public delegate void ExitMonitorEventHandler(Node2D body);
	[Signal] public delegate void DiedEventHandler();
	[Signal] public delegate void DyingEventHandler();

	public Player player = null;
	public override void _Ready()
	{
		MonitorArea.BodyEntered += OnMonitorAreaBodyEntered;
		MonitorArea.BodyExited += OnMonitorAreaBodyExited;
		ChaseArea.BodyEntered += OnChaseAreaBodyEntered;
		ChaseArea.BodyExited += OnChaseAreaBodyExited;
		AttackArea.BodyEntered += OnAttackAreaBodyEntered;
		AttackArea.BodyExited += OnAttackAreaBodyExited;

		Stats.GetStat("Health").StatChanged += OnHealthChanged;

		HealthBar.MaxValue = Stats.GetStatValue("Health");
		HealthBar.Value = HealthBar.MaxValue;
	}

	public void OnMonitorAreaBodyEntered(Node2D body)
	{
		EmitSignal(SignalName.EnterMonitor, body);
	}
	public void OnMonitorAreaBodyExited(Node2D body)
	{
		EmitSignal(SignalName.ExitMonitor, body);
	}
	public void OnChaseAreaBodyEntered(Node2D body)
	{
		EmitSignal(SignalName.EnterChase, body);
	}
	public void OnChaseAreaBodyExited(Node2D body)
	{
		EmitSignal(SignalName.ExitChase, body);
	}
	public void OnAttackAreaBodyEntered(Node2D body)
	{
		EmitSignal(SignalName.EnterAttack, body);
	}
	public void OnAttackAreaBodyExited(Node2D body)
	{
		EmitSignal(SignalName.ExitAttack, body);
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	public void OnHealthChanged(float oldValue, float newValue)
	{
		if (newValue < oldValue)
		{
			GetHit();
			if (IsInsideTree())
			{
				FloatingText Text = FloatingTextScene.Instantiate<FloatingText>();
				GetTree()?.CurrentScene?.AddChild(Text);
				Text.GlobalPosition = GlobalPosition + new Vector2(GD.RandRange(-5, 5), GD.RandRange(-30, -15));
				Text.display((int)(oldValue - newValue));
			}
		}
		if (newValue <= 0)
			Die();

		if (HealthBar != null)
			UpdateHealthBar(newValue);
	}
	private void UpdateHealthBar(float newValue)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(HealthBar, "value", newValue, 0.1f)
			.SetTrans(Tween.TransitionType.Linear)
			.SetEase(Tween.EaseType.InOut);
	}
	public async void Die()
	{
		EmitSignal(SignalName.Dying);
		await OnDeath();
		DropCoin();
		EmitSignal(SignalName.Died);
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.EnemyDied, GlobalPosition);
		QueueFree();
	}
	public void GetHit()
	{
		OnHit();
		Flash();
	}
	protected virtual async Task OnDeath() => await Task.Delay(0);
	protected virtual void OnHit() {}
	public virtual void CustomBehaviour(Player player) { }
	private void DropCoin()
    {
		bool willDrop = false;
		Probability.RunIfElse(CoinDropRate, () => willDrop = true, () => { });
		if (!willDrop)
			return;
		int coinsToDrop = GD.RandRange(MinCoinDrop, MaxCoinDrop);
		int coinOnGroundAmount = (int)Mathf.Log(coinsToDrop) * 2 + 1;
		int coinsInCoinBoost = coinsToDrop / coinOnGroundAmount;
		int remainder = coinsToDrop % coinOnGroundAmount;
		float spread = Mathf.Pi / 6;
		float force = 200f;
		for (int i = 0; i <= coinOnGroundAmount; i++)
		{
			float direction = (float)GD.RandRange(-Mathf.Pi / 2 - spread, -Mathf.Pi / 2 + spread);
			Boost coin = ResourceLoader.Load<PackedScene>("res://Boosts/Special/Coin.tscn").Instantiate<Boost>();
			if (i == coinOnGroundAmount)
				coin.Info.Amount = remainder;
			else
				coin.Info.Amount = coinsInCoinBoost;

			if (IsInsideTree())
			{
				coin.Position = Position;
				GetTree()?.CurrentScene?.AddChild(coin);
				coin.ApplyCentralImpulse(Vector2.Right.Rotated(direction) * force);
			}
		}
    }
	private void Flash()
	{
		if (Sprite == null)
			return;

		Tween _tween = CreateTween();
		_tween.TweenProperty(
			Sprite, "modulate",
			new Color(1, 0.5f, 0.5f),
			0.1f
		).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
		_tween.TweenProperty(
			Sprite, "modulate",
			new Color(1, 1, 1),
			0.3f
		).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
	}
}
