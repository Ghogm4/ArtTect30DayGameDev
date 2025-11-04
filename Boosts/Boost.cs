using Godot;
using System;
using System.Collections.Generic;

public partial class Boost : RigidBody2D
{
    [Export] public BoostInfo Info = null;
    [Export] public bool DisplayWhenObtained = true;
    [Export] public bool DisplayOnCurrentBoosts = true;
    public bool Pickable
    {
        get => field;
        set
        {
            field = value;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.BoostPickableFieldChanged, value);
        }
    } = true;
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
        Info = Info.Duplicate(true) as BoostInfo;
    }
    public void DoBoost(StatComponent statComponent)
    {
        int amount = Info.Amount;
        for (int i = 0; i < amount; i++)
            _modifierComponent.ModifyStatComponent(statComponent);
        if (!DropTable.ObtainedOneTimeBoosts.Contains(SceneFilePath) && Info.IsOneTimeOnly)
        {
            DropTable.ObtainedOneTimeBoosts.Add(SceneFilePath);
            GD.Print($"Registered one-time boost in global one-time boost list: {SceneFilePath}");
        }
    }
}
