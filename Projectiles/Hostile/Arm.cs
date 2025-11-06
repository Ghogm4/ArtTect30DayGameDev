using Godot;
using System;
using System.Collections;

public partial class Arm : Projectile
{
	[Export] public Area2D ExpireArea = null;
	[Export] public Area2D ExplodeArea = null;
	[Export] public PackedScene ArmExplodeEffectScene = null;
	[Export] public float AccelerationMagnitude = 500f;
	[Export] public float LifeTime = 5f;
	public Vector2 Velocity = Vector2.Zero;
	public Vector2 Acceleration = Vector2.Zero;
	public int Damage = 0;
	protected Player _player = null;
	private float _speed = 0f;

	public override void _EnterTree()
	{
		_speed = Velocity.Length();
	}
	private void RegisterExplosionOnWorld()
    {
        ExpireArea.BodyEntered += (body) =>
		{
			if (body is TileMapLayer || (body.Get("collision_layer").AsInt32() & 1) == 1)
				Explode();
		};
    }
	protected override void ReadyBehavior()
	{
		RegisterExplosionOnWorld();
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		GetTree().CreateTimer(LifeTime).Timeout += () =>
		{
			if (IsInstanceValid(this))
				Explode();
		};
	}
	public override void _Process(double delta)
	{
		Acceleration = (_player.GlobalPosition - GlobalPosition).Normalized() * AccelerationMagnitude;
		Velocity += Acceleration * (float)delta;
		Velocity = Velocity.Normalized() * _speed;
		Rotation = Velocity.Angle();
		Position += Velocity * (float)delta;
	}
	protected override void HitBehavior(Node2D body)
	{
		if (body is Player player)
			Explode();
	}
	protected void Explode()
	{
		foreach (var body in ExplodeArea.GetOverlappingBodies())
			if (body is Player player)
				player.TakeDamage(Damage, Callable.From<Player>((player) => { }));

		var explosion = ArmExplodeEffectScene.Instantiate<ArmExplodeEffect>();
		explosion.GlobalPosition = GlobalPosition;
		GetTree().CurrentScene.AddChild(explosion);
		ExtraExplodeBehavior();
		QueueFree();
	}
	protected virtual void ExtraExplodeBehavior() {}
}
