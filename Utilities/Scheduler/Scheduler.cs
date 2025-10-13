using Godot;
using System;
using System.Collections.Generic;

public partial class Scheduler : Node
{
    public static Scheduler Instance;
    public override void _Ready() => Instance = this;
    public void ScheduleAction(float delay, Action action, bool finalizeBeforeSceneChange = false)
    {
        SceneTreeTimer timer = GetTree().CreateTimer(delay);
        timer.Timeout += () => action?.Invoke();
        if (finalizeBeforeSceneChange)
        {
            Action emitTimeout = () => timer.EmitSignal(SceneTreeTimer.SignalName.Timeout);
            SignalBus.Instance.RegisterSceneChangeStartedAction(emitTimeout, SignalBus.Priority.Medium);
            timer.Timeout += () => SignalBus.Instance.RemoveSceneChangeStartedAction(emitTimeout);
        }
    }
}
