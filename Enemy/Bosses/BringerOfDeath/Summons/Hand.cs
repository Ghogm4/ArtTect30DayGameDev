using Godot;
using System;

public partial class Hand : AnimatedSprite2D
{
	[Export] public float PlaySpeed = 1f;
	[Export] public AnimationPlayer AttackAnimationPlayer;
	[Export] public Area2D AttackArea;
	private bool HeadingLeft => _player.GlobalPosition.X <= GlobalPosition.X;
	private Player _player;
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
		AttackAnimationPlayer.Play("Stretch", -1, 1.1f);
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
		tween.TweenProperty(this, "modulate:a", 0f, 0.3f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
