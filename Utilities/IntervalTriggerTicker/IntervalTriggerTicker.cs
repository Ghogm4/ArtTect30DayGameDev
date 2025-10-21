using Godot;
using System;
using System.Collections.Generic;

public partial class IntervalTriggerTicker : Node
{
    public static IntervalTriggerTicker Instance;
    private List<IntervalTrigger> _triggers = new();
    private List<IntervalTrigger> _triggersToRemove = new();
    public override void _Ready() => Instance = this;
    public void RegisterTrigger(IntervalTrigger trigger)
    {
        if (!_triggers.Contains(trigger))
            _triggers.Add(trigger);
    }
    public override void _Process(double delta)
    {
        foreach (var trigger in _triggers)
        {
            trigger.Tick(delta);
            if (trigger.IsCompleted)
                _triggersToRemove.Add(trigger);
        }
        foreach (var trigger in _triggersToRemove)
            _triggers.Remove(trigger);
    
        _triggersToRemove.Clear();
    }
}
