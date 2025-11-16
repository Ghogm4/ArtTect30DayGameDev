using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class Stat : Resource
{
    [Signal] public delegate void StatChangedEventHandler(float oldValue, float newValue);
    [Signal] public delegate void StatReachedMinEventHandler();
    [Signal] public delegate void StatReachedMaxEventHandler();
    [Export] public string Name = string.Empty;
    [Export] public float BaseValue { get; private set; } = 0f;
    // Enable this for frequently modified stats to merge all normal modifiers into the three types when the count exceeds threshold
    [Export] public bool Mergeable = false;
    // Enable this to allow instant calculation upon modification, suitable for those needed to be shown on UI
    [Export] public bool CalculateOnModify = false;
    [Export] public string MinValue = string.Empty;
    [Export] public string MaxValue = string.Empty;
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
    public StatLimit MinLimit = new();
    public StatLimit MaxLimit = new();
    public bool DoLimitValidation = false;
    private float _cachedValue = -1f;
    private bool _needRefresh = true;
    private List<StatModifier> _modifiers = new();
    private StatModifier _lastAddedModifier = null;
    private (float, float, float) HandleModifiers()
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
        return (baseAdd, mult, finalAdd);
    }
    public void Calculate(float referencedStatOldValue = 0, float referencedStatNewValue = 0)
    {
        if (Mergeable && _normalModifierCount > NormalModifierThreshold)
            MergeNormalModifiers();
        float oldValue = _cachedValue;

        (float baseAdd, float mult, float finalAdd) = HandleModifiers();
        float _calculatedValue = (BaseValue + baseAdd) * mult + finalAdd;
        if (DoLimitValidation && _lastAddedModifier != null)
        {
            bool _isLastAddedModifierValid =
            HandleCalculationExceedMinValidation(_calculatedValue, baseAdd, mult) &
            HandleCalculationExceedMaxValidation(_calculatedValue, baseAdd, mult);
            if (!_isLastAddedModifierValid) {
                Calculate(referencedStatOldValue, referencedStatNewValue);
                return;
            }
        }
        GD.PrintErr($"Stat '{Name}' calculated value {_calculatedValue} is within limits.");
        _cachedValue = _calculatedValue;
        _needRefresh = false;
        EmitSignal(SignalName.StatChanged, oldValue, _cachedValue);
    }
    private void CancelLastAddedModifier()
    {
        if (_lastAddedModifier == null)
            return;
        RemoveModifier(_lastAddedModifier);
        _lastAddedModifier = null;
    }
    private bool HandleCalculationExceedMinValidation(float calculatedValue, float baseAdd, float mult)
    {
        float minVal = MinLimit.Resolve();
        if (minVal < calculatedValue || Mathf.IsEqualApprox(minVal, calculatedValue) ||_lastAddedModifier == null)
            return true;
        GD.PrintErr($"Stat '{Name}' calculated value {calculatedValue} is below min limit {minVal}.");
        float _lastAddedModifierValue = _lastAddedModifier.Value;
        StatModifier.OperationType _lastAddedModifierType = _lastAddedModifier.Type;
        CancelLastAddedModifier();
        if (_lastAddedModifierType is StatModifier.OperationType.BaseAdd)
        {
            float neededAdd = (minVal - calculatedValue) / mult + _lastAddedModifierValue;
            AddBase(neededAdd);
        }
        else if (_lastAddedModifierType is StatModifier.OperationType.FinalAdd)
        {
            float neededAdd = _lastAddedModifierValue - calculatedValue + minVal;
            AddFinal(neededAdd);
        }
        else
        {
            float neededMult = (minVal - calculatedValue) / ((BaseValue + baseAdd) * mult) + _lastAddedModifierValue;
            Mult(neededMult);
        }
        EmitSignal(SignalName.StatReachedMin);

        return false;
    }
    private bool HandleCalculationExceedMaxValidation(float calculatedValue, float baseAdd, float mult)
    {
        float maxVal = MaxLimit.Resolve();
        if (maxVal > calculatedValue || Mathf.IsEqualApprox(maxVal, calculatedValue) || _lastAddedModifier == null)
            return true;
        float _lastAddedModifierValue = _lastAddedModifier.Value;
        StatModifier.OperationType _lastAddedModifierType = _lastAddedModifier.Type;
        CancelLastAddedModifier();
        if (_lastAddedModifierType is StatModifier.OperationType.BaseAdd)
        {
            float neededAdd = _lastAddedModifierValue - (calculatedValue - maxVal) / maxVal;
            AddBase(neededAdd);
        }
        else if (_lastAddedModifierType is StatModifier.OperationType.FinalAdd)
        {
            float neededAdd = maxVal + _lastAddedModifierValue - calculatedValue;
            AddFinal(neededAdd);
        }
        else
        {
            float neededMult = (maxVal - calculatedValue) / ((BaseValue + baseAdd) * mult) + _lastAddedModifierValue;
            Mult(neededMult);
        }
        EmitSignal(SignalName.StatReachedMax);

        return false;
    }
    // This method will merge even non-normal modifiers into three types
    public void MergeNormalModifiers()
    {
        _normalModifierCount = 0;
        (float baseAdd, float mult, float finalAdd) = HandleModifiers();
        _modifiers.Clear();
        int types = 0;
        if (!Mathf.IsZeroApprox(baseAdd))
        {
            AddBase(baseAdd);
            types++;
        }
        if (!Mathf.IsZeroApprox(mult - 1f))
        {
            Mult(mult);
            types++;
        }
        if (!Mathf.IsZeroApprox(finalAdd))
        {
            AddFinal(finalAdd);
            types++;
        }
        _normalModifierCount = types;
        _needRefresh = true;
    }
    public void AddModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        if (IsUselessModifier(modifier))
            return;
        _modifiers.Add(modifier);
        _needRefresh = true;
        _lastAddedModifier = modifier;
        if (CalculateOnModify)
            Calculate();
        if (modifier.ReferencedStat == null)
            _normalModifierCount++;
    }
    public void Reset()
    {
        _modifiers.Clear();
        Calculate();
        _normalModifierCount = 0;
        _lastAddedModifier = null;
    }
    private bool IsUselessModifier(StatModifier modifier)
    {
        return
        (
            (modifier.Type == StatModifier.OperationType.BaseAdd || modifier.Type == StatModifier.OperationType.FinalAdd)
                && Mathf.IsZeroApprox(modifier.Value)
        ) ||
        (
            modifier.Type == StatModifier.OperationType.Mult && Mathf.IsEqualApprox(modifier.Value, 1f)
        );
    }
    public void RemoveModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        _modifiers.Remove(modifier);
        _needRefresh = true;

        if (_lastAddedModifier == modifier)
            _lastAddedModifier = null;
        if (CalculateOnModify)
            Calculate();
        if (modifier.ReferencedStat == null)
            _normalModifierCount--;
    }
    public List<StatModifierResource> CreateModifierResources()
    {
        var resources = new List<StatModifierResource>();
        foreach (var modifier in _modifiers)
        {
            StatModifierResource resource = new();
            resource.Type = modifier.Type;
            if (modifier.ReferencedStat != null)
            {
                resource.ReferencedStatName = modifier.ReferencedStat.Name;
                resource.ReferencedPercentage = modifier.Value / modifier.ReferencedStat.FinalValue * 100f;
            }
            else
            {
                resource.Value = modifier.Value;
            }
            resources.Add(resource);
        }
        _modifiers.Clear();
        return resources;
    }
    public void AddBase(float value) => AddModifier(StatModifier.Factory.BaseAdd(value));
    public void Mult(float value) => AddModifier(StatModifier.Factory.Mult(value));
    public void AddFinal(float value) => AddModifier(StatModifier.Factory.FinalAdd(value));
    public void SetValue(float value)
    {
        float difference = value - FinalValue;
        AddFinal(difference);
    }
}

public class StatLimit
{
    public Func<float> ValueProvider = null;
    public float Resolve() => ValueProvider != null ? ValueProvider() : 0f;
}