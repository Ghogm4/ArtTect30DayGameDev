using Godot;
using System;
using System.Collections;

public partial class BlueOrb : Projectile
{
	public Vector2 Velocity = Vector2.Zero;
	public bool PierceWorld = false;
	private bool _isExpired = false;
    protected override void ReadyBehavior()
    {
        if (!PierceWorld)
        {
			Hitbox.BodyEntered += body =>
			{
				if (body is TileMapLayer || (body.Get("collision_layer").AsInt32() & 1) == 1)
					RunExpireAnimation();
			};
        }
    }
    protected override void HitBehavior(Node2D body)
    {
        if (body is Player player)
		{
			player.TakeDamage(1, Callable.From<Player>((player) => { }));
			RunExpireAnimation();
		}
    }
	public override void _Process(double delta)
	{
		Position += Velocity * (float)delta;
		Velocity = Velocity.MoveToward(Vector2.Zero, 300f * (float)delta);
		if (Mathf.IsZeroApprox(Velocity.Length()))
			RunExpireAnimation();
	}
	private void RunExpireAnimation()
	{
		if (_isExpired)
			return;
		_isExpired = true;
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.3f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
