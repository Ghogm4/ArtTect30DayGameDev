using Godot;
using System;

public partial class Reaper : AnimatedSprite2D
{
	[Export] public float Speed = 120f;
	[Export] public float LifeTime = 3f;
	[Export] public AnimationPlayer AttackAnimationPlayer;
	[Export] public Area2D AttackArea;
	[Export] public CollisionShape2D AttackCollisionShape;
	private bool HeadingLeft => _player.GlobalPosition.X <= GlobalPosition.X;
	private Player _player;
	private bool _startedAttack = false;
	private bool _didInitialDetect = false;
	private void Appear()
	{
		Modulate = Modulate with { A = 0 };
		Tween tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 1f, 0.3f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
	}
	public override void _Ready()
	{
		Appear();
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		AttackAnimationPlayer.AnimationFinished += OnAnimationFinished;
		AttackArea.BodyEntered += body =>
		{
			if (body is not Player || _startedAttack) return;
			AttackAnimationPlayer.Play("Attack");
			_startedAttack = true;
		};
		GetTree().CreateTimer(LifeTime).Timeout += () =>
		{
			if (_startedAttack || !IsInstanceValid(this)) return;
			_startedAttack = true;
			OnAnimationFinished(null);
		};
	}
	public override void _Process(double delta)
	{
		if (_startedAttack) return;
		FlipH = HeadingLeft;
		AttackArea.Scale = HeadingLeft ? new(-1, 1) : Vector2.One;
		Vector2 direction = (_player.GlobalPosition - AttackCollisionShape.GlobalPosition).Normalized();
		Position += direction * Speed * (float)delta;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (_didInitialDetect) return;
		_didInitialDetect = true;
		foreach (var body in AttackArea.GetOverlappingBodies())
			if (body is Player)
				AttackAnimationPlayer.Play("Attack");
	}
	private void ChangeFacing()
	{
		FlipH = !HeadingLeft;
		AttackArea.Scale = HeadingLeft ? new Vector2(-1, 1) : Vector2.One;
	}
	private void Attack()
	{
		foreach (var body in AttackArea.GetOverlappingBodies())
			if (body is Player player)
				player.TakeDamage(1, Callable.From<Player>(player => { }));
	}
	private void OnAnimationFinished(StringName str)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 0f, 1f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
