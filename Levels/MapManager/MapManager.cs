using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class MapManager : Node2D
{
	public static MapManager Instance { get; private set; }
	[Export] public Godot.Collections.Array<PackedScene> MapPool;
	[Signal] public delegate void MapGeneratedEventHandler();
	[Signal] public delegate void MapChangedEventHandler();
	public List<Map> Maps = new();
	public List<Map> EnabledMaps = new List<Map>();
	public List<Map> EndNodeMaps = new List<Map>();
	public string Entrance = null;
	public Map NowMap = null;
	public Map StartMap = null;
	public Map EndMap = null;
	private bool _isEndCreated = false;
	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
			ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			QueueFree();
		}

		SignalBus.Instance.EntranceSignal += OnEntranceEntered;
	}
	private void Reset()
	{
		Maps = new();
		EnabledMaps.Clear();
		EndNodeMaps.Clear();
		NowMap = null;
		StartMap = null;
		EndMap = null;
		_isEndCreated = false;
	}
	public async void OnEntranceEntered(string entrance)
	{
		Map TargetMap = NowMap.GetMap(entrance);
		Entrance = entrance;
		if (TargetMap == null)
		{
			GD.PrintErr("TargetMap is null.");
			return;
		}

		SceneManager.Instance.ChangeScene(TargetMap.Scene);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);

		if (GetTree().CurrentScene is BaseLevel newLevel)
			newLevel.InitializeLevel(TargetMap.Position);
		
		NowMap = TargetMap;

		SetPlayerPosition(entrance);
		
		if (!NowMap.IsDiscovered)
			NowMap.IsDiscovered = true;
		
		EmitSignal(SignalName.MapChanged);
		MapALG.Instance.PrintMap(NowMap.Position);
	}

	public void SetPlayerPosition(string entrance)
	{
		var current = GetTree().CurrentScene;
		var player = GetTree().GetNodesInGroup("Player").FirstOrDefault() as Player;
		var baseLevel = current as BaseLevel;
		if (current == null || player == null || baseLevel == null)
		{
			CallDeferred(MethodName.SetPlayerPosition, entrance);
			return;
		}

		switch (entrance)
		{
			case "Top":
				if (baseLevel.BottomMarker == null) { CallDeferred(MethodName.SetPlayerPosition, entrance); return; }
				player.GlobalPosition = baseLevel.BottomMarker.GlobalPosition;
				break;
			case "Bottom":
				if (baseLevel.TopMarker == null) { CallDeferred(MethodName.SetPlayerPosition, entrance); return; }
				player.GlobalPosition = baseLevel.TopMarker.GlobalPosition;
				break;
			case "Left":
				if (baseLevel.RightMarker == null) { CallDeferred(MethodName.SetPlayerPosition, entrance); return; }
				player.GlobalPosition = baseLevel.RightMarker.GlobalPosition;
				break;
			case "Right":
				if (baseLevel.LeftMarker == null) { CallDeferred(MethodName.SetPlayerPosition, entrance); return; }
				player.GlobalPosition = baseLevel.LeftMarker.GlobalPosition;
				break;
			case "Start":
				if (baseLevel.StartMarker == null) { CallDeferred(MethodName.SetPlayerPosition, entrance); return; }
				player.GlobalPosition = baseLevel.StartMarker.GlobalPosition;
				break;
			default:
				GD.PrintErr("Invalid entrance for setting player position: " + entrance);
				break;
		}
	}
	public Map SearchMap(MapType type)
	{
		GD.Print("Searching for map of type: " + type.ToString());
		List<Map> filteredMaps = Maps.Where(m => m.Type == type && !m.IsEnabled && !m.IsEndLevel && !m.IsStartLevel).ToList();
		if (filteredMaps.Count > 0)
		{
			Random random = new Random();
			int index = random.Next(filteredMaps.Count);
			return filteredMaps[index];
		}
		return null;
	}

	public void InitMaps()
	{
		Reset();
		if (Maps != null)
			Maps.ForEach(map => map.IsEnabled = false);
		
		Maps = new List<Map>();
		EnabledMaps = new List<Map>();
		foreach (PackedScene scene in MapPool)
		{
			var mapLevel = scene.Instantiate<BaseLevel>();
			if (mapLevel == null)
			{
				GD.PrintErr("MapLevel is null.");
				continue;
			}
			Map newMap = new Map(
				scene,
				Vector2I.Left,
				mapLevel.TopExit,
				mapLevel.BottomExit,
				mapLevel.LeftExit,
				mapLevel.RightExit,
				mapLevel.IsStartLevel,
				mapLevel.IsEndLevel,
				mapLevel.RarityWeight
				);
			Maps.Add(newMap);
			if (newMap.IsStartLevel)
				StartMap = newMap;
			if (newMap.IsEndLevel)
				EndMap = newMap;

			GD.Print("Loaded map: " + scene.ResourcePath);
		}

		MapALG.Instance.InitMap();
		MapALG.Instance.StartRoom();
		MapALG.Instance.PrintMap();
		ApplyMap();
		EmitSignal(SignalName.MapGenerated);
	}
    public async void StartLevel()
    {
        NowMap = StartMap;
        StartMap.IsDiscovered = true;
        EmitSignal(SignalName.MapChanged);
        SceneManager.Instance.ChangeScene(NowMap.Scene);
        await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);

        if (GetTree().CurrentScene is BaseLevel newLevel)
        {
            newLevel.InitializeLevel(NowMap.Position);
        }

        SetPlayerPosition("Start");
    }
    public void ApplyMap()
    {
		var assigned = new Dictionary<Vector2I, Map>();

		foreach (var room in MapALG.Instance.Roomlist)
		{
			if (!room.IsEnabled) continue;
			MapType desiredType = room.JudgeMapType();
			Map chosen = SearchMap(desiredType);
			if (chosen == null)
			{
				GD.PrintErr("No available map found for type: " + desiredType.ToString());
				continue;
			}
			if (room.Position == MapALG.Instance.startPos)
				chosen = StartMap;
			
			chosen.IsEnabled = true;
			chosen.Position = room.Position;
			assigned[room.Position] = chosen;
			EnabledMaps.Add(chosen);
		}

		foreach (var pair in assigned)
		{
			var room = MapALG.Instance.GetMapAtPosition(pair.Key);
			var map = pair.Value;
			if (room.TopExit)
			{
				var key = new Vector2I(room.Position.X, room.Position.Y - 1);
				if (assigned.ContainsKey(key))
				{
					map.TopMap = assigned[key];
					assigned[key].BottomMap = map;
					GD.Print("Connected ", map.Position, " top to ", assigned[key].Position);
				}
			}
			if (room.BottomExit)
			{
				var key = new Vector2I(room.Position.X, room.Position.Y + 1);
				if (assigned.ContainsKey(key))
				{
					map.BottomMap = assigned[key];
					assigned[key].TopMap = map;
					GD.Print("Connected ", map.Position, " bottom to ", assigned[key].Position);
				}
			}
			if (room.LeftExit)
			{
				var key = new Vector2I(room.Position.X - 1, room.Position.Y);
				if (assigned.ContainsKey(key))
				{
					map.LeftMap = assigned[key];
					assigned[key].RightMap = map;
					GD.Print("Connected ", map.Position, " left to ", assigned[key].Position);
				}
			}
			if (room.RightExit)
			{
				var key = new Vector2I(room.Position.X + 1, room.Position.Y);
				if (assigned.ContainsKey(key))
				{
					map.RightMap = assigned[key];
					assigned[key].LeftMap = map;
					GD.Print("Connected ", map.Position, " right to ", assigned[key].Position);
				}
			}

			if (room.GetExitCount() == 1 && !EndNodeMaps.Contains(map) && map != StartMap)
			{
				EndNodeMaps.Add(map);
			}
		}
		Vector2I sp = new(MapALG.Instance.startPos.X, MapALG.Instance.startPos.Y);
		if (assigned.TryGetValue(sp, out var startMap))
		{
			StartMap = startMap;
			StartMap.IsStartLevel = true;
			NowMap = StartMap;
		}
		var rng = new Random();
		int index = rng.Next(EndNodeMaps.Count);
		Map node = EndNodeMaps[index];

		EndMap.Position = node.Position;
		EndMap.TopExit = node.TopExit;
		EndMap.BottomExit = node.BottomExit;
		EndMap.LeftExit = node.LeftExit;
		EndMap.RightExit = node.RightExit;
		EndMap.TopMap = node.TopMap;
		EndMap.BottomMap = node.BottomMap;
		EndMap.LeftMap = node.LeftMap;
		EndMap.RightMap = node.RightMap;
		EndMap.IsEnabled = true;
		if (!EnabledMaps.Contains(EndMap)) EnabledMaps.Add(EndMap);

        foreach (var map in EnabledMaps)
        {
            if (map.TopMap == node) map.TopMap = EndMap;
            if (map.BottomMap == node) map.BottomMap = EndMap;
            if (map.LeftMap == node) map.LeftMap = EndMap;
            if (map.RightMap == node) map.RightMap = EndMap;
        }

        node.IsEnabled = false;
		EnabledMaps.Remove(node);
	}

	public Map GetMapAtPosition(Vector2I position)
	{
		return EnabledMaps.FirstOrDefault(m => m.Position == position);
	}
}
