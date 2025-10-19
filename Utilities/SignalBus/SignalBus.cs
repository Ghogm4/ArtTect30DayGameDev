using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SignalBus : Node
{
    [Signal] public delegate void PlayerHitEventHandler(int damage, Callable customBehavior);
    [Signal] public delegate void PlayerHealthStatusUpdatedEventHandler(int health, int maxHealth, int shield);
    [Signal] public delegate void PlayerDiedEventHandler();
    [Signal] public delegate void ShowTextEventHandler();
    [Signal] public delegate void WaitAdvanceEventHandler();
    [Signal] public delegate void SceneChangeStartedEventHandler();
    [Signal] public delegate void PlayerBoostPickedUpEventHandler(BoostInfo info, bool needDisplay);

    [Signal] public delegate void EntranceSignalEventHandler(string entrance);
    public static SignalBus Instance { get; private set; }
    public enum Priority
    {
        Low = 0, Medium = 10, High = 20, Super = 30
    }
    private List<Tuple<int, Action>> _onSceneChangeStartedActions = new();
    public override void _Ready()
    {
        Instance = this;
        SceneChangeStarted += TriggerSceneChangeStartedActions;
    }
    public void RegisterSceneChangeStartedAction(Action action, int priority)
    {
        _onSceneChangeStartedActions.Add(new Tuple<int, Action>(priority, action));
    }
    public void RemoveSceneChangeStartedAction(Action action)
    {
        var tuple = _onSceneChangeStartedActions.FirstOrDefault(x => x.Item2 == action);
        if (tuple != default)
            _onSceneChangeStartedActions.Remove(tuple);

    }
    public void RegisterSceneChangeStartedAction(Action action, Priority priority)
    {
        _onSceneChangeStartedActions.Add(new Tuple<int, Action>((int)priority, action));
    }
    private void TriggerSceneChangeStartedActions()
    {
        var orderedActions = _onSceneChangeStartedActions.OrderByDescending(x => x.Item1);
        foreach (var tuple in orderedActions)
            tuple.Item2?.Invoke();
        _onSceneChangeStartedActions.Clear();
    }

    /*
    Registered Actions / Priority:
        Disconnect the method OnPlayerHit in Player_UniversalState from PlayerHit signal, 30
        Player buff removal, 10
        Finalize all dash recovery timers, 10
        Transfer stat modifiers of player's StatComponent into GameData's StatModifierDict, 0
        Make PlayerHealthBar visible when entering game from Start New Journey -> Yes and Load Save, 0
        Make PlayerHealthBar invisible when exiting to title, 0
    */
}
