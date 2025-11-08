using Godot;
using System;

public partial class DarkEssence : Projectile
{
    [Export] public float Gravity = 100f;
    public Vector2 Velocity = Vector2.Zero;
    public float Damage = 1;
    private bool _isExpired = false;
    protected override void ReadyBehavior()
    {
        Scale = Vector2.Zero;
        Tween tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.One, 0.2f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        Hitbox.BodyEntered += (body) =>
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
        velocity.Y = Mathf.Clamp(velocity.Y + Gravity * (float)delta, -1000f, 1000f);
        Velocity = velocity;
    }
    protected override void HitBehavior(Node2D body)
    {
        if (_isExpired) return;
        if (body is Player player)
        {
            player.TakeDamage(Damage, Callable.From<Player>(player => {}));
        }
    }
    private void RunExpireAnimation()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.Zero, 0.2f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
