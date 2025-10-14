using Godot;
using System;
using System.Collections.Generic;

public partial class Scheduler : Node
{
    public static Scheduler Instance;
    public override void _Ready() => Instance = this;
    public void ScheduleAction(float delay, Action action, int priority, bool finalizeBeforeSceneChange = false)
    {
        SceneTreeTimer timer = GetTree().CreateTimer(delay);
        timer.Connect(SceneTreeTimer.SignalName.Timeout, Callable.From(() => action?.Invoke()), (uint)ConnectFlags.OneShot);
        if (finalizeBeforeSceneChange)
        {
            Action emitTimeout = () => timer.EmitSignal(SceneTreeTimer.SignalName.Timeout);
            SignalBus.Instance.RegisterSceneChangeStartedAction(emitTimeout, priority);
            timer.Timeout += () => SignalBus.Instance.RemoveSceneChangeStartedAction(emitTimeout);
        }
    }
}
