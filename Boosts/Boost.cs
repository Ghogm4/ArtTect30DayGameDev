using Godot;
using System;
using System.Collections.Generic;

public partial class Boost : RigidBody2D
{
    [Export] public BoostInfo Info = null;
    [Export] public bool NeedDisplay = true;
    private StatModifierComponent _modifierComponent = null;
    public override void _Ready()
    {
        var component = GetNode<StatModifierComponent>("StatModifierComponent");
        if (component is null)
        {
            GD.PushError("Boost must have a direct child of StatModifierComponent.");
            return;
        }
        _modifierComponent = component;
    }
    public void DoBoost(StatComponent statComponent)
    {
        int amount = Info.Amount;
        for (int i = 0; i < amount; i++)
            _modifierComponent.ModifyStatComponent(statComponent);
    }
}
