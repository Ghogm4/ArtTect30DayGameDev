using Godot;
using System;
using System.Collections;

public partial class ArmExplodeEffect : AnimatedSprite2D
{
	public override void _Ready()
	{
		AnimationFinished += QueueFree;
    }
}
