using Godot;
using System;

public partial class PickupArea : Area2D
{
	[Export] public StatComponent StatComponent;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	public void OnBodyEntered(Node2D body)
	{
		if (body is Boost boost)
		{
			boost.DoBoost(StatComponent);
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerBoostPickedUp, boost.Info, boost.NeedDisplay);
			boost.Free();
		}
	}
}
