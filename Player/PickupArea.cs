using Godot;
using System;

public partial class PickupArea : Area2D
{
	[Export] public StatComponent StatComponent;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		SignalBus.Instance.BoostPickableFieldChanged += OnBoostPickableFieldChanged;
		SignalBus.Instance.RegisterSceneChangeStartedAction(() =>
		{
			SignalBus.Instance.BoostPickableFieldChanged -= OnBoostPickableFieldChanged;
		}, SignalBus.Priority.Low);
	}
	public void OnBodyEntered(Node2D body)
	{
		if (body is Boost boost && boost.Pickable)
		{
			boost.DoBoost(StatComponent);
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerBoostPickedUp, boost.Info, boost.NeedDisplay);
			Tween tween = boost.CreateTween();
			tween.TweenProperty(boost, "scale", Vector2.Zero, 0.3f)
				.SetTrans(Tween.TransitionType.Quad)
				.SetEase(Tween.EaseType.In);
			tween.TweenCallback(Callable.From(boost.QueueFree));
		}
	}
	private void OnBoostPickableFieldChanged(bool pickable)
	{
		if (!pickable) return;
		foreach (Node2D body in GetOverlappingBodies())
			OnBodyEntered(body);
	}
}
