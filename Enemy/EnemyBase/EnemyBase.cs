using Godot;
using System;

public partial class EnemyBase : CharacterBody2D
{
	[Export] public Area2D MonitorArea;
	[Export] public Area2D AttackArea;
	[Export] public Area2D ChaseArea;
	[Export] public VarStorage Storage;
	[Export] public AnimatedSprite2D Sprite;

	[Signal] public delegate void EnterMonitorEventHandler(Node2D body);
	[Signal] public delegate void EnterChaseEventHandler(Node2D body);
	[Signal] public delegate void EnterAttackEventHandler(Node2D body);
	[Signal] public delegate void ExitAttackEventHandler(Node2D body);
	[Signal] public delegate void ExitChaseEventHandler(Node2D body);
	[Signal] public delegate void ExitMonitorEventHandler(Node2D body);

	public Player player = null;
	public override void _Ready()
	{
		MonitorArea.BodyEntered += OnMonitorAreaBodyEntered;
		MonitorArea.BodyExited += OnMonitorAreaBodyExited;
		ChaseArea.BodyEntered += OnChaseAreaBodyEntered;
		ChaseArea.BodyExited += OnChaseAreaBodyExited;
		AttackArea.BodyEntered += OnAttackAreaBodyEntered;
		AttackArea.BodyExited += OnAttackAreaBodyExited;
	}

	public void OnMonitorAreaBodyEntered(Node2D body)
	{
		EmitSignal("EnterMonitor", body);
	}
	public void OnMonitorAreaBodyExited(Node2D body)
	{
		EmitSignal("ExitMonitor", body);
	}
	public void OnChaseAreaBodyEntered(Node2D body)
	{
		EmitSignal("EnterChase", body);
	}
	public void OnChaseAreaBodyExited(Node2D body)
	{
		EmitSignal("ExitChase", body);
	}
	public void OnAttackAreaBodyEntered(Node2D body)
	{
		EmitSignal("EnterAttack", body);
	}
	public void OnAttackAreaBodyExited(Node2D body)
	{
		EmitSignal("ExitAttack", body);
	}

	public override void _PhysicsProcess(double delta)
	{
		this.MoveAndSlide();
	}

}
