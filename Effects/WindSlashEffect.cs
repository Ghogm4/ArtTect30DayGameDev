using Godot;
using System;
using System.Collections;

public partial class WindSlashEffect : AnimatedSprite2D
{
    public override void _Ready()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.One * 2f, 0.1f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
        tween.TweenProperty(this, "scale", Vector2.Zero, 0.3f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        AnimationFinished += QueueFree;
    }
}
