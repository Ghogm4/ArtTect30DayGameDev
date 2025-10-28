using Godot;
using System;

public partial class FloatingText : Node2D
{
	public Tween position_tween;
	public Tween scale_tween;
	public Label label = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");

		Scale = new Vector2(1f, 1f);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Display(float damage)
	{
		if (position_tween != null && position_tween.IsRunning())
		{
			position_tween.Stop();
		}
		if (scale_tween != null && scale_tween.IsRunning())
		{
			scale_tween.Stop();
		}

		label.Text = damage.ToString();
		position_tween = CreateTween(); 
		scale_tween = CreateTween();

		position_tween.TweenProperty(
			this, "position",
			Position + new Vector2(0, -3f),
			0.3f
		).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

		scale_tween.TweenProperty(
			this, "scale",
			new Vector2(0.7f, 0.7f),
			0.2f
		).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

		scale_tween.TweenProperty(
			this, "scale",
			new Vector2(0.5f, 0.5f),
			0.1f
		).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

		position_tween.TweenProperty(
			this, "position",
			Position + new Vector2(GD.RandRange(-4, 4), -5f),
			0.2f
		).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

		scale_tween.TweenProperty(
			this, "scale",
			new Vector2(0f, 0f),
			0.2f
		).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

		position_tween.TweenCallback(Callable.From(() => QueueFree()));
		scale_tween.TweenCallback(Callable.From(() => QueueFree()));
	}
}
