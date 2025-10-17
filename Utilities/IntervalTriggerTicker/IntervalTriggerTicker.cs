using Godot;
using System;
using System.Collections.Generic;

public partial class IntervalTriggerTicker : Node
{
    public static IntervalTriggerTicker Instance;
    private List<IntervalTrigger> _triggers = new();
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
                _triggers.Remove(trigger);
        }
    }
}
