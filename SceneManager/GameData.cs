using Godot;
using System;
using System.Collections.Generic;
using GodotDictionary = Godot.Collections.Dictionary;
public partial class GameData : Node
{
    public static GameData Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        SignalBus.Instance.PlayerStatResetRequested += Reset;
    }
    public Dictionary<string, List<StatModifierResource>> StatModifierDict = new();
    public List<IntervalTrigger> PlayerPassiveSkills = new();
    public List<Action<StatComponent, PlayerStatComponent>> PlayerAttackActions = new();
    public List<Action<PlayerStatComponent, Vector2>> PlayerOnEnemyDeathActions = new();
    public bool PlayerStatComponentInitialized = false;
    public Dictionary<Vector2I, GodotDictionary> MapStates { get; set; } = new();
    public void Reset()
    {
        StatModifierDict.Clear();
        PlayerPassiveSkills.Clear();
        PlayerAttackActions.Clear();
        PlayerOnEnemyDeathActions.Clear();
        PlayerStatComponentInitialized = false;
        MapStates.Clear();
    }

    public GodotDictionary GetRoomState(Vector2I roomPos)
    {
        if (!MapStates.ContainsKey(roomPos))
            MapStates[roomPos] = new GodotDictionary();
    
        return MapStates[roomPos];
    }
    public void SaveObjectState(Vector2I roomPos, string objectID, GodotDictionary state)
    {
        GetRoomState(roomPos)[objectID] = state;
    }

    public GodotDictionary LoadObjectState(Vector2I roomPos, string objectID)
    {
        var roomState = GetRoomState(roomPos);
        
        if (roomState.ContainsKey(objectID))
            return roomState[objectID].AsGodotDictionary();
        
        GD.PushError($"No saved state for objectID: {objectID} in room position: {roomPos}. Is it the first time entering this room?");
        return null;
    }
}
