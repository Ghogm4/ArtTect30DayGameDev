using Godot;
using System;
using System.Collections;

public partial class WindSlashEffect : AnimatedSprite2D
{
	public override void _Ready()
    {
		AnimationFinished += QueueFree;
    }
}
