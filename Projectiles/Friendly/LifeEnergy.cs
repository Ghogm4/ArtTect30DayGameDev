using Godot;
using System;

public partial class LifeEnergy : Projectile
{
	[Export] public float LifeTime = 3f;
	[Export] public float AccelerationMagnitude = 200f;
	[Export] public int TrailFrequency = 2;
	[Export] public float TrailLifeTime = 0.5f;
	[Export] public Area2D HomingArea = null;
	[Export] public PackedScene TrailScene;
	private EnemyBase _targetEnemy = null;
	public Vector2 Velocity = Vector2.Zero;
	public float Damage = 0;
	private bool _updated = false;
	private float _speed = 300f;
	private int _frameCounter = 0;
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
		_speed = Velocity.Length();
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
	private void GenerateTrail()
	{
		Trail trail = TrailScene.Instantiate<Trail>();
		trail.GlobalPosition = GlobalPosition;
		trail.Texture = GetNode<Sprite2D>("Sprite2D").Texture;
		trail.LifeTime = TrailLifeTime;
		GetTree().CurrentScene.CallDeferred(MethodName.AddChild, trail);
	}
	public override void _Process(double delta)
	{
		if (IsExpired) return;
		_frameCounter++;
		if (_frameCounter >= TrailFrequency)
		{
			GenerateTrail();
			_frameCounter = 0;
		}
		Vector2 velocity = Velocity;
		Vector2 acceleration = Vector2.Zero;
		if (_targetEnemy != null && IsInstanceValid(_targetEnemy))
		{
			Vector2 direction = (_targetEnemy.GlobalPosition - GlobalPosition).Normalized();
			acceleration = direction * AccelerationMagnitude;
		}
		else
		{
			velocity = velocity.Lerp(Vector2.Zero, 0.03f);
		}
		velocity += acceleration * (float)delta;
		velocity = velocity.Normalized() * Mathf.Min(velocity.Length(), _speed);
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
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.One * 2f, 0.3f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.3f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
