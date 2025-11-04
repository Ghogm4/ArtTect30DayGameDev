using Godot;
using System;

public partial class PickupArea : Area2D
{
	[Export] public StatComponent StatComponent;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		SignalBus.Instance.BoostPickableFieldChanged += OnBoostPickableFieldChanged;
	}
	public override void _ExitTree()
	{
		BodyEntered -= OnBodyEntered;
		SignalBus.Instance.BoostPickableFieldChanged -= OnBoostPickableFieldChanged;
	}
	public void OnBodyEntered(Node2D body)
	{
		if (body is Boost boost && boost.Pickable)
		{
			boost.DoBoost(StatComponent);
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerBoostPickedUp, boost.Info, boost.DisplayWhenObtained, boost.DisplayOnCurrentBoosts);
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
