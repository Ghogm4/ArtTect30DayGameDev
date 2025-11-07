using Godot;
using System;

public partial class Trail : Sprite2D
{
    public float LifeTime = 0.5f;
    public override void _Ready()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0f, LifeTime).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
