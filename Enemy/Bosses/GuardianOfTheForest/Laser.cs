using Godot;
using System;

public partial class Laser : RayCast2D
{
	[Export] public float MaxDistance = 400f;
	[Export] public float LaserInnerWidth = 8f;
	[Export] public PackedScene BlueOrbScene = null;
	[Export] public float OrbSpawnInterval = 0.2f;
	[Export] public float OrbSpeed = 150f;
	private Area2D LaserDamageArea => GetNode<Area2D>("LaserDamageArea");
	private Vector2 HitPos => IsColliding() ? ToLocal(GetCollisionPoint()) : TargetPosition;
	private Tween _laserInnerAppearTween = null;
	private Tween _laserOuterAppearTween = null;
	private bool _isFiring = false;
	private float _timeElapsed = 0f;
	public bool Appearing
	{
		get => field;
		set
		{
			if (field == value) return;
			field = value;
			_laserInnerAppearTween = CreateTween();
			_laserOuterAppearTween = CreateTween();
			if (field)
			{
				_laserInnerAppearTween.TweenProperty(_laserInnerLine, "width", LaserInnerWidth, 1f);
				_laserOuterAppearTween.TweenProperty(_laserOuterLine, "width", LaserInnerWidth * 1.5, 1f);
				_laserOuterAppearTween.TweenCallback(Callable.From(() => LaserDamageArea.Monitoring = true));
				_isFiring = true;
			}
			else
			{
				LaserDamageArea.Monitoring = false;
				_laserInnerAppearTween.TweenProperty(_laserInnerLine, "width", 0f, 0.5f);
				_laserOuterAppearTween.TweenProperty(_laserOuterLine, "width", 0f, 0.5f);
				_isFiring = false;
			}
		}
	} = false;
	private Line2D _laserInnerLine = null;
	private Line2D _laserOuterLine = null;

	public override void _Ready()
	{
		TargetPosition = Vector2.Zero;
		_laserInnerLine = GetNode<Line2D>("LaserInnerLine");
		_laserOuterLine = GetNode<Line2D>("LaserOuterLine");
		_laserInnerLine.Width = 0f;
		_laserInnerLine.SetPointPosition(1, Vector2.Zero);
		_laserOuterLine.Width = 0f;
		_laserOuterLine.SetPointPosition(1, Vector2.Zero);
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 targetPosition = TargetPosition;
		if (!_isFiring)
			targetPosition.X = Mathf.MoveToward(targetPosition.X, 0, 1000f * (float)delta);
		else
			targetPosition.X = Mathf.MoveToward(targetPosition.X, MaxDistance, 1000f * (float)delta);
		TargetPosition = targetPosition;
		ForceRaycastUpdate();
		LaserDamageArea.Position = HitPos / 2;
		RectangleShape2D rect = LaserDamageArea.GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;
		rect.Size = new Vector2(HitPos.X, 6f);
		_laserInnerLine.SetPointPosition(1, HitPos);
		_laserOuterLine.SetPointPosition(1, HitPos);
		TryDoLaserDamage();
	}
	public override void _Process(double delta)
    {
		if (!IsColliding() && !Mathf.IsEqualApprox(TargetPosition.X, MaxDistance)) return;
		_timeElapsed += (float)delta;
		if (_timeElapsed >= OrbSpawnInterval)
        {
			_timeElapsed -= OrbSpawnInterval;
			SpawnOrb(OrbSpeed, (float)GD.RandRange(0f, Mathf.Tau), true);
        }
    }
	private void SpawnOrb(float speed, float radian, bool canPierceWorld)
	{
		var orb = BlueOrbScene.Instantiate<BlueOrb>();
		orb.GlobalPosition = ToGlobal(HitPos);
		orb.Velocity = Vector2.Right.Rotated(radian) * speed;
		orb.Rotation = orb.Velocity.Angle();
		orb.PierceWorld = canPierceWorld;
		GetTree().CurrentScene.CallDeferred(MethodName.AddChild, orb);
	}
	private void TryDoLaserDamage()
	{
		if (!LaserDamageArea.Monitoring) return;
		foreach (var body in LaserDamageArea.GetOverlappingBodies())
			if (body is Player player)
				player.TakeDamage(1f, Callable.From<Player>(player => { }));
	}
}