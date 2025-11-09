using Godot;
using System;

public partial class DarkEssence : Projectile
{
    public Vector2 Velocity = Vector2.Zero;
    public float Damage = 1;
    private bool _isExpired = false;
    private Vector2 _originalScale;
    protected override void ReadyBehavior()
    {
        _originalScale = Scale;
        Scale = Vector2.Zero;
        Tween tween = CreateTween();
        tween.TweenProperty(this, "scale", _originalScale, 0.2f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
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
        Position += Velocity * (float)delta;
        Velocity = Velocity.Lerp(Vector2.Zero, (float)delta * 4f);
        if (Velocity.Length() < 1f)
        {
            _isExpired = true;
            RunExpireAnimation();
        }
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
