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
    private float _cachedValue = -1f;
    private bool _needRefresh = true;
    private List<StatModifier> _modifiers = new();
    private StatModifier _lastAddedModifier = null;
    public void Calculate(float referencedStatOldValue = 0, float referencedStatNewValue = 0)
    {
        if (Mergeable && _normalModifierCount > NormalModifierThreshold)
            MergeNormalModifiers();
        float oldValue = _cachedValue;
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

        float _calculatedValue = (BaseValue + baseAdd) * mult + finalAdd;

        bool _isLastAddedModifierValid =
        HandleCalculationExceedMinValidation(_calculatedValue, baseAdd, mult) &&
        HandleCalculationExceedMaxValidation(_calculatedValue, baseAdd, mult);
        if (!_isLastAddedModifierValid)
        {
            RemoveModifier(_lastAddedModifier);
            return;
        }

        _cachedValue = _calculatedValue;
        _needRefresh = false;
        EmitSignal(SignalName.StatChanged, oldValue, _cachedValue);
    }
    private bool HandleCalculationExceedMinValidation(float _calculatedValue, float baseAdd, float mult)
    {
        float minVal = MinLimit.Resolve();
        if (minVal <= _calculatedValue)
            return true;

        if (_lastAddedModifier.Type is StatModifier.OperationType.BaseAdd)
        {
            float neededAdd = (minVal - _calculatedValue) / mult + _lastAddedModifier.Value;
            AddBase(neededAdd);
        }
        else if (_lastAddedModifier.Type is StatModifier.OperationType.FinalAdd)
        {
            float neededAdd = _lastAddedModifier.Value - _calculatedValue + minVal;
            AddFinal(neededAdd);
        }
        else
        {
            float neededMult = (minVal - _calculatedValue) / ((BaseValue + baseAdd) * mult) + _lastAddedModifier.Value;
            Mult(neededMult);
        }
        EmitSignal(SignalName.StatReachedMin);
        return false;
    }
    private bool HandleCalculationExceedMaxValidation(float _calculatedValue, float baseAdd, float mult)
    {
        float maxVal = MaxLimit.Resolve();
        if (maxVal >= _calculatedValue)
            return true;

        if (_lastAddedModifier.Type is StatModifier.OperationType.BaseAdd)
        {
            float neededAdd = _lastAddedModifier.Value - (_calculatedValue - maxVal) / maxVal;
            AddBase(neededAdd);
        }
        else if (_lastAddedModifier.Type is StatModifier.OperationType.FinalAdd)
        {
            float neededAdd = maxVal + _lastAddedModifier.Value - _calculatedValue;
            AddFinal(neededAdd);
        }
        else
        {
            float neededMult = (maxVal - _calculatedValue) / ((BaseValue + baseAdd) * mult) + _lastAddedModifier.Value;
            Mult(neededMult);
        }
        EmitSignal(SignalName.StatReachedMax);
        return false;
    }
    public void MergeNormalModifiers()
    {
        _normalModifierCount = 0;
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
}

public class StatLimit
{
    public Func<float> ValueProvider = null;
    public float Resolve() => ValueProvider != null ? ValueProvider() : 0f;
}