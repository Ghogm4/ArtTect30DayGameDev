using Godot;
using System;
using System.Collections.Generic;
using GDDictionary = Godot.Collections.Dictionary;
public partial class GameData : Node
{
    public static GameData Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        SignalBus.Instance.PlayerStatResetRequested += Reset;
    }
    public float TotalPlayTimeInSeconds = 0f;
    public int TotalBoostsCollected = 1;
    public bool VictoryAchieved = false;
    public Dictionary<string, List<StatModifierResource>> StatModifierDict = new();
    public List<Action<EnemyBase, PlayerStatComponent>> PlayerOnHittingEnemyActions = new();
    public List<IntervalTrigger> PlayerPassiveSkills = new();
    public List<Action<EnemyBase, PlayerStatComponent>> PlayerOnEnemyDeathActions = new();
    public List<Action<PlayerStatComponent, Vector2>> PlayerOnAttackActions = new();
    public List<Action<PlayerStatComponent, Vector2>> PlayerOnJumpActions = new();
    public List<Action<PlayerStatComponent, Vector2>> PlayerOnDashActions = new();
    public List<Func<float, PlayerStatComponent, float>> PlayerDamageCalculators = new();
    public bool PlayerStatComponentInitialized = false;
    public Dictionary<Vector2I, GDDictionary> MapStates { get; set; } = new();
    public override void _Process(double delta)
    {
        TotalPlayTimeInSeconds += (float)delta;
    }

    public void Reset()
    {
        TotalPlayTimeInSeconds = 0;
        TotalBoostsCollected = 1;
        VictoryAchieved = false;
        StatModifierDict.Clear();
        PlayerPassiveSkills.Clear();
        PlayerOnHittingEnemyActions.Clear();
        PlayerOnEnemyDeathActions.Clear();
        PlayerOnAttackActions.Clear();
        PlayerOnJumpActions.Clear();
        PlayerOnDashActions.Clear();
        PlayerDamageCalculators.Clear();
        PlayerStatComponentInitialized = false;
        MapStates.Clear();
    }

    public GDDictionary GetRoomState(Vector2I roomPos)
    {
        if (!MapStates.ContainsKey(roomPos))
            MapStates[roomPos] = new GDDictionary();
    
        return MapStates[roomPos];
    }
    public void SaveObjectState(Vector2I roomPos, string objectID, GDDictionary state) =>
        GetRoomState(roomPos)[objectID] = state;

    public GDDictionary LoadObjectState(Vector2I roomPos, string objectID)
    {
        var roomState = GetRoomState(roomPos);
        
        if (roomState.ContainsKey(objectID))
            return roomState[objectID].AsGodotDictionary();

        GD.PushWarning($"No saved state for objectID: {objectID} in room position: {roomPos}. Is it the first time entering this room?");
        return null;
    }
}
