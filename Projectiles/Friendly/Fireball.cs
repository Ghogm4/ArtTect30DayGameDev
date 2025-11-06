using Godot;
using System;
using System.Threading.Tasks;

public partial class Fireball : Projectile
{
	[Export] public float LifeTime = 3f;
	[Export] public float AccelerationMagnitude = 200f;
	[Export] public Area2D HomingArea = null;
	[Export] public PackedScene FireballEffectScene = null;
	private EnemyBase _targetEnemy = null;
	public Vector2 Velocity = Vector2.Zero;
	public float Damage = 0;
	private bool _updated = false;
	private bool IsExpired
	{
		get => field;
		set
		{
			if (field == value) return;
			field = value;
			if (field)
				RunExpireAnimation();
		}
	} = false;
	protected override void ReadyBehavior()
	{
		GetTree().CreateTimer(LifeTime).Timeout += () =>
		{
			if (IsInstanceValid(this))
				IsExpired = true;
		};
	}
	private void UpdateTargetEnemy()
	{
		float closestDistance = float.MaxValue;
		foreach (var body in HomingArea.GetOverlappingBodies())
		{
			if (body is EnemyBase enemy && !enemy.IsDead)
			{
				float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					_targetEnemy = enemy;
				}
			}
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		if (!_updated)
		{
			UpdateTargetEnemy();
			if (_targetEnemy != null)
				_updated = true;
		}
	}
	public override void _Process(double delta)
	{
		if (IsExpired) return;
		Vector2 velocity = Velocity;
		Vector2 acceleration = Vector2.Zero;
		if (_targetEnemy != null && IsInstanceValid(_targetEnemy))
		{
			Vector2 direction = (_targetEnemy.GlobalPosition - GlobalPosition).Normalized();
			acceleration = direction * AccelerationMagnitude;
		}
		velocity += acceleration * (float)delta;
		velocity = velocity.Normalized() * Mathf.Min(velocity.Length(), BaseSpeed);
		Position += velocity * (float)delta;
		Rotation = velocity.Angle();
		Velocity = velocity;
	}
	protected override void HitBehavior(Node2D body)
	{
		if (IsExpired) return;
		if (body is EnemyBase enemy)
		{
			enemy.TakeDamage(Damage);
			IsExpired = true;
		}
	}
	private void RunExpireAnimation()
	{
		FireballEffect effect = FireballEffectScene.Instantiate<FireballEffect>();
		GetTree().CurrentScene.AddChild(effect);
		effect.GlobalPosition = GlobalPosition;
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
