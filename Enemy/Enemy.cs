using Godot;
using System;
using System.Net.NetworkInformation;

public partial class Enemy : CharacterBody2D
{
	public int Health = 100;
	public int ChaseSpeed = 100;
	public int MaxFallSpeed = 500;
	public Player _player = null;
	[Export] private VarStorage Storage;
	[Export] private Area2D MonitorArea;
	[Export] private Area2D DetectArea;
	[Export] private Area2D AttackArea;

	[Signal] public delegate void EnterMonitorEventHandler(Node2D body);
	[Signal] public delegate void EnterDetectEventHandler(Node2D body);
	[Signal] public delegate void EnterAttackEventHandler(Node2D body);
	[Signal] public delegate void LeaveAttackEventHandler(Node2D body);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Storage.RegisterVariant<Vector2>("PlayerPosition", Vector2.Zero);

		MonitorArea.BodyEntered += OnMonitorAreaBodyEntered;
		DetectArea.BodyEntered += OnDetectAreaBodyEntered;
		AttackArea.BodyEntered += OnAttackAreaBodyEntered;
		AttackArea.BodyExited += OnAttackAreaBodyExited;
		_player = GetParent().GetNode<Player>("Player");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 playerPos = _player.GlobalPosition;
		Storage.SetVariant("PlayerPosition", playerPos);
	}

	public override void _PhysicsProcess(double delta)
	{
		this.MoveAndSlide();
	}

	public void OnMonitorAreaBodyEntered(Node2D body)
	{
		if (body is Player)
		{
			GD.Print("I can see player");
		}
		EmitSignal("EnterMonitor", body);
	}
	public void OnDetectAreaBodyEntered(Node2D body)
	{
		if (body is Player)
		{
			GD.Print("I can detect player");
		}
	}
	public void OnAttackAreaBodyEntered(Node2D body)
	{
		if (body is Player)
		{
			GD.Print("I can attack player");
			EmitSignal("EnterAttack", body);
		}
	}
	public void OnAttackAreaBodyExited(Node2D body)
	{
		if (body is Player)
		{
			GD.Print("Player left attack area");
			EmitSignal("LeaveAttack", body);
		}
	}
}
