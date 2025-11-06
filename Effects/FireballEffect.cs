using Godot;
using System;

public partial class FireballEffect : AnimatedSprite2D
{
	public override void _Ready()
    {
        AnimationFinished += QueueFree;
    }
}
