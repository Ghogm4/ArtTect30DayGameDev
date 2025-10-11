using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class Stat : Resource
{
    [Signal] public delegate void StatChangedEventHandler();
    [Export] public float BaseValue { get; private set; } = 0f;
    // Enable this for frequently modified stats to merge all normal modifiers into the three types when the count exceeds threshold
    [Export] public bool Mergeable = false;
    private const int NormalModifierThreshold = 20;
    private int _normalModifierCount = 0;
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
        if (Mergeable && _normalModifierCount > NormalModifierThreshold)
            MergeNormalModifiers();
            
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
    public void MergeNormalModifiers()
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

        _modifiers.Clear();
        int types = 0;
        if (!Mathf.IsZeroApprox(baseAdd))
        {
            _modifiers.Add(StatModifier.Factory.BaseAdd(baseAdd));
            types++;
        }
        if (!Mathf.IsZeroApprox(mult - 1f))
        {
            _modifiers.Add(StatModifier.Factory.Mult(mult));
            types++;
        }
        if (!Mathf.IsZeroApprox(finalAdd))
        {
            _modifiers.Add(StatModifier.Factory.FinalAdd(finalAdd));
            types++;
        }
        _normalModifierCount = types;
        _needRefresh = true;
    }
    public void AddModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        _modifiers.Add(modifier);
        _needRefresh = true;

        if (modifier.ReferencedStat == null)
            _normalModifierCount++;
    }
    public void RemoveModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        _modifiers.Remove(modifier);
        _needRefresh = true;

        if (modifier.ReferencedStat == null)
            _normalModifierCount--;
    }
    public void AddBase(float value) => AddModifier(StatModifier.Factory.BaseAdd(value));
    public void Mult(float value) => AddModifier(StatModifier.Factory.Mult(value));
    public void AddFinal(float value) => AddModifier(StatModifier.Factory.FinalAdd(value));
}
