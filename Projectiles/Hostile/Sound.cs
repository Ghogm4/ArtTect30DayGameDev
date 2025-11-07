using Godot;
using System;
using static Godot.GD;
public partial class Sound : Projectile
{
	[Export] public int TrailFrequency = 4;
	[Export] public float LifeTime = 2f;
	[Export] public Sprite2D SoundSprite;
	[Export] public PackedScene TrailScene;
	public Vector2 Velocity = Vector2.Zero;
	private int _frameCounter = 0;
	private bool IsExpired
	{
		get => field;
		set
		{
			if (field == value) return;
			field = value;
			if (field)
			{
				Expire();
			}
		}
	} = false;
	protected override void ReadyBehavior()
	{
		Hitbox.BodyEntered += (body) =>
		{
			if (body is TileMapLayer || (body.Get("collision_layer").AsInt32() & 1) == 1)
				IsExpired = true;
		};
		GetTree().CreateTimer(LifeTime).Timeout += Expire;
	}
	protected override void HitBehavior(Node2D body)
	{
		if (body is Player player)
		{
			player.TakeDamage(1, Callable.From<Player>((player) => { }));
			Expire();
		}
	}
	public override void _Process(double delta)
	{
		_frameCounter++;
		if (_frameCounter >= TrailFrequency)
		{
			Trail trail = TrailScene.Instantiate<Trail>();
			trail.GlobalPosition = GlobalPosition;
			trail.Texture = SoundSprite.Texture;
			GetTree().CurrentScene.CallDeferred(MethodName.AddChild, trail);
			_frameCounter = 0;
		}
		Position += Velocity * (float)delta;
		Velocity = Velocity.Lerp(Vector2.Zero, 0.5f * (float)delta);
	}
	private void Expire()
	{
		CallDeferred(MethodName.DisableCollision);
		RunExpireAnimation();
	}
	private void DisableCollision()
	{
		Hitbox.Monitoring = false;
	}
	private void RunExpireAnimation()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.2f).SetTrans(Tween.TransitionType.Quart).SetEase(Tween.EaseType.Out);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
