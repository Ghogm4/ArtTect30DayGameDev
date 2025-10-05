using Godot;
using System;

public partial class ResponsiveButton : Button
{
	private const float _hoverScale = 1.1f;
	private Vector2 _scaleVector = Vector2.Zero;
	private const float _duration = 0.1f;
	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		_scaleVector = new Vector2(_hoverScale, _hoverScale);
		PivotOffset = Size / 2;
	}
	private void OnMouseEntered()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", _scaleVector, _duration)
			.SetTrans(Tween.TransitionType.Quart)
			.SetEase(Tween.EaseType.Out);
	}
	private void OnMouseExited()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.One, _duration)
			.SetTrans(Tween.TransitionType.Quart)
			.SetEase(Tween.EaseType.Out);
	}
}
