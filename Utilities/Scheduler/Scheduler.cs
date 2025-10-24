using Godot;
using System;
using System.Collections.Generic;

public partial class Scheduler : Node
{
    public static Scheduler Instance;
    public override void _Ready()
    {
        Instance = this;
        SignalBus.Instance.PlayerStatResetRequested += FinalizeAllTimers;
    }
    private List<SceneTreeTimer> _timers = new();
    public void ScheduleAction(float delay, Action action, int priority, bool finalizeBeforeSceneChange = false)
    {
        SceneTreeTimer timer = GetTree().CreateTimer(delay);
        _timers.Add(timer);
        timer.Connect(SceneTreeTimer.SignalName.Timeout, Callable.From(() =>
        {
            action?.Invoke();
            _timers.Remove(timer);
        }), (uint)ConnectFlags.OneShot);
        
        if (finalizeBeforeSceneChange)
        {
            Action emitTimeout = () => timer?.EmitSignal(SceneTreeTimer.SignalName.Timeout);
            SignalBus.Instance.RegisterSceneChangeStartedAction(emitTimeout, priority);
            timer.Timeout += () => SignalBus.Instance.RemoveSceneChangeStartedAction(emitTimeout);
        }
    }
    public void FinalizeAllTimers()
    {
        foreach (SceneTreeTimer timer in _timers)
            timer.Dispose();
        _timers.Clear();
    }
}
