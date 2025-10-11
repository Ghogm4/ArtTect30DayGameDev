using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class Stat : Resource
{
    [Signal] public delegate void StatChangedEventHandler();
    [Export] public float BaseValue { get; private set; } = 0f;
    public float FinalValue
    {
        get
        {
            if (_needRefresh)
                Calculate();
            return _cachedValue;
        }
    }
    private float _cachedValue = -1f;
    private bool _needRefresh = true;
    private List<StatModifier> _modifiers = new();
    public void Calculate()
    {
        float baseAdd = 0f;
        float mult = 1f;
        float finalAdd = 0f;
        foreach (var modifier in _modifiers)
        {
            switch (modifier.Type)
            {
                case StatModifier.OperationType.BaseAdd:
                    baseAdd = modifier.Operate(baseAdd);
                    break;
                case StatModifier.OperationType.Mult:
                    mult = modifier.Operate(mult);
                    break;
                case StatModifier.OperationType.FinalAdd:
                    finalAdd = modifier.Operate(finalAdd);
                    break;
            }
        }
        _cachedValue = (BaseValue + baseAdd) * mult + finalAdd;
        _needRefresh = false;
        EmitSignal(SignalName.StatChanged);
    }
    public void AddModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        _modifiers.Add(modifier);
        _needRefresh = true;
    }
    public void RemoveModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        _modifiers.Remove(modifier);
        _needRefresh = true;
    }
}
