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
		if (body.IsInGroup("Pickups"))
		{
			StatModifierComponent statModifierComponent = body.GetNode<StatModifierComponent>("StatModifierComponent");
			statModifierComponent.ModifyStatComponent(StatComponent);
			body.QueueFree();
		}
	}
}
