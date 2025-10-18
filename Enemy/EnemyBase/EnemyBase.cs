using Godot;
using System;

public partial class EnemyBase : CharacterBody2D
{
	[Export] public Area2D MonitorArea;
	[Export] public Area2D AttackArea;
	[Export] public Area2D ChaseArea;
	[Export] public VarStorage Storage;
	[Export] public AnimatedSprite2D Sprite;
	[Export] public StatComponent Stats;
	[Export] public TextureProgressBar HealthBar; 

	[Signal] public delegate void EnterMonitorEventHandler(Node2D body);
	[Signal] public delegate void EnterChaseEventHandler(Node2D body);
	[Signal] public delegate void EnterAttackEventHandler(Node2D body);
	[Signal] public delegate void ExitAttackEventHandler(Node2D body);
	[Signal] public delegate void ExitChaseEventHandler(Node2D body);
	[Signal] public delegate void ExitMonitorEventHandler(Node2D body);

	private StatWrapper _health;

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

		_health = new(Stats.GetStat("Health"));
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

	public virtual void CustomBehaviour(Player player)
	{

	}

	public async void OnHealthChanged()
	{
		_health = new(Stats.GetStat("Health"));
		if (HealthBar != null)
		{
			double currentValue = HealthBar.Value; // 当前血条值
			double targetValue = (double)_health;  // 目标血条值

			Tween tween = CreateTween(); // 创建 Tween 动画
			tween.TweenProperty(HealthBar, "value", targetValue, 0.1f) // 插值到目标值，持续 0.5 秒
				.SetTrans(Tween.TransitionType.Linear)
				.SetEase(Tween.EaseType.InOut);

			await ToSignal(tween, "finished"); // 等待动画完成
		}
	}
}
