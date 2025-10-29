using Godot;
using System;

public partial class WindSlash : Projectile
{
	[Export] public int PierceEnemyCount = 2;
	[Export] public Area2D ExpireArea = null;
	public Vector2 Velocity = Vector2.Zero;
	public float Damage = 0;
	private bool _isExpired = false;
	protected override void ReadyBehavior()
	{
		ExpireArea.BodyEntered += (body) =>
		{
			if (body is TileMapLayer || (body.Get("collision_layer").AsInt32() & 1) == 1)
			{
				_isExpired = true;
				RunExpireAnimation();
			}
		};
	}
	public override void _Process(double delta)
	{
		if (_isExpired) return;
		Vector2 velocity = Velocity;
		Position += velocity * (float)delta;
		velocity.X = Mathf.Lerp(velocity.X, 0, 0.15f);
		velocity.Y = Mathf.Lerp(velocity.Y, 0, 0.15f);
		Velocity = velocity;
		float tolerance = 1f;
		if (Mathf.IsEqualApprox(Velocity.Length(), 0f, tolerance))
		{
			_isExpired = true;
			RunExpireAnimation();
		}
	}
	protected override void HitBehavior(Node2D body)
	{
		GD.Print("Hit body: " + body.Name);
		if (_isExpired) return;
		if (body is EnemyBase enemy)
		{
			enemy.TakeDamage(Damage);
			PierceEnemyCount--;
			PlayEffect();
		}
		if (PierceEnemyCount == 0)
		{
			_isExpired = true;
			RunExpireAnimation();
		}
	}
	private void RunExpireAnimation()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
		tween.TweenCallback(Callable.From(QueueFree));
	}
	private void PlayEffect()
	{
		WindSlashEffect effect = ResourceLoader.Load<PackedScene>("res://Effects/WindSlashEffect.tscn").Instantiate<WindSlashEffect>();
		GetTree().CurrentScene.AddChild(effect);
		effect.GlobalPosition = GlobalPosition;
	}
}
