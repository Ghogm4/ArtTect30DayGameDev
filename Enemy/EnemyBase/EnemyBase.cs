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

	[Export] public PackedScene FloatingTextScene;
	
	[Signal] public delegate void EnterMonitorEventHandler(Node2D body);
	[Signal] public delegate void EnterChaseEventHandler(Node2D body);
	[Signal] public delegate void EnterAttackEventHandler(Node2D body);
	[Signal] public delegate void ExitAttackEventHandler(Node2D body);
	[Signal] public delegate void ExitChaseEventHandler(Node2D body);
	[Signal] public delegate void ExitMonitorEventHandler(Node2D body);

	private StatWrapper _health;
	private float preHealth;
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
		preHealth = (float)_health;
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
		if ((float)_health < preHealth)
		{
			GD.Print("dis");
			FloatingText Text = FloatingTextScene.Instantiate<FloatingText>();
			AddChild(Text);
			Text.GlobalPosition = GlobalPosition + new Vector2(0, -20);
			Text.display((int)(preHealth - (float)_health));
		}
		if (HealthBar != null)
		{
			double currentValue = HealthBar.Value;
			double targetValue = (double)_health;

			Tween tween = CreateTween();
			tween.TweenProperty(HealthBar, "value", targetValue, 0.1f)
				.SetTrans(Tween.TransitionType.Linear)
				.SetEase(Tween.EaseType.InOut);

			await ToSignal(tween, "finished");
		}
		preHealth = (float)_health;
	}
}
