using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class MapManager : Node2D
{
	public static MapManager Instance { get; private set; }
	[Export] public Godot.Collections.Array<PackedScene> ForestMapPool;
	[Export] public Godot.Collections.Array<PackedScene> KingdomMapPool;
	[Export] public Godot.Collections.Array<PackedScene> DungeonMapPool;
	// [Export] public Godot.Collections.Array<PackedScene> ExtraMapPool;
	[Signal] public delegate void MapGeneratedEventHandler();
	[Signal] public delegate void MapChangedEventHandler();
	public Godot.Collections.Array<PackedScene> TargetMapPool
	{
		get
		{
			return MapPoolIndex switch
			{
				0 => ForestMapPool,
				1 => KingdomMapPool,
				2 => DungeonMapPool,
				_ => default
			};
		}
	}
	public List<Map> Maps = new();
	public List<Map> ExtraMaps = new();
	public List<Map> EnabledMaps = new List<Map>();
	public List<Map> EndNodeMaps = new List<Map>();
	public int MapPoolIndex = 0;
	public string Entrance = null;
	public Map NowMap = null;
	public Map StartMap = null;
	public Map EndMap = null;
	private bool _isEndCreated = false;
	private Vector2 _returnPos = Vector2.Zero;
	private Vector2I _returnMapPos = Vector2I.Left;

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
	private void InitializeSavedMap(Vector2I mapPos)
	{
		if (GetTree().CurrentScene is BaseLevel nowLevel)
			nowLevel.InitializeLevel(mapPos);
		GD.Print("MapManager: Initialized saved map at position: " + mapPos);
	}
	public async void ReturnToMainMap()
	{
		if (_returnMapPos == Vector2I.Left)
			return;
		Map mapToReturn = GetMapAtPosition(_returnMapPos);
		if (mapToReturn != null)
			NowMap = mapToReturn;
		SceneManager.Instance.ChangeScene(NowMap.Scene);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		GD.Print("MapManager: Returned to main map at position: " + NowMap.Position);
		InitializeSavedMap(NowMap.Position);
		Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
		if (player == null) CallDeferred(MethodName.ReturnToMainMap);
		player.GlobalPosition = _returnPos;
	}
	public void RecordReturnPosition(Vector2 portalPos)
	{
		_returnPos = portalPos;
		_returnMapPos = NowMap.Position;
	}
	private void Reset()
	{
		Maps.Clear();
		EnabledMaps.Clear();
		EndNodeMaps.Clear();
		NowMap = null;
		StartMap = null;
		EndMap = null;
		_isEndCreated = false;
	}
	public void InitMaps()
	{
		Reset();
		if (Maps != null)
			Maps.ForEach(map => map.IsEnabled = false);
		GD.Print("TargetMapPool Count: " + TargetMapPool.Count);
		foreach (PackedScene scene in TargetMapPool)
		{
			var mapLevel = scene.Instantiate<BaseLevel>();
			if (mapLevel == null)
			{
				GD.PushError("MapLevel is null.");
				continue;
			}
			Map newMap = new Map(
				scene, Vector2I.Left,
				mapLevel.TopExit, mapLevel.BottomExit, mapLevel.LeftExit, mapLevel.RightExit,
				mapLevel.IsStartLevel, mapLevel.IsEndLevel,
				mapLevel.RarityWeight
				);
			Maps.Add(newMap);
			if (newMap.IsStartLevel)
				StartMap = newMap;

			GD.Print("Loaded map: " + scene.ResourcePath);
		}
		// foreach (PackedScene scene in ExtraMapPool)
		// {
		// 	var mapLevel = scene.Instantiate<BaseLevel>();
		// 	if (mapLevel == null)
		// 	{
		// 		GD.PushError("Extra MapLevel is null.");
		// 		continue;
		// 	}
		// 	Map newMap = new Map(scene);
		// 	newMap.IsExtraLevel = true;
		// 	ExtraMaps.Add(newMap);

		// 	GD.Print("Loaded extra map: " + scene.ResourcePath);
		// }
	}
	public async void OnEntranceEntered(string entrance)
	{
		Map TargetMap = NowMap.GetMap(entrance);
		Entrance = entrance;
		if (TargetMap == null)
		{
			GD.PushError("TargetMap is null.");
			return;
		}

		SceneManager.Instance.ChangeScene(TargetMap.Scene);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);

		InitializeSavedMap(TargetMap.Position);

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
		bool isMarkerFound = true;
		switch (entrance)
		{
			case "Start":
				if (baseLevel.StartMarker == null) break;
				player.GlobalPosition = baseLevel.StartMarker.GlobalPosition;
				isMarkerFound = true;
				break;
			case "Top":
				if (baseLevel.BottomMarker == null) break;
				player.GlobalPosition = baseLevel.BottomMarker.GlobalPosition;
				isMarkerFound = true;
				break;
			case "Bottom":
				if (baseLevel.TopMarker == null) break;
				player.GlobalPosition = baseLevel.TopMarker.GlobalPosition;
				isMarkerFound = true;
				break;
			case "Left":
				if (baseLevel.RightMarker == null) break;
				player.GlobalPosition = baseLevel.RightMarker.GlobalPosition;
				isMarkerFound = true;
				break;
			case "Right":
				if (baseLevel.LeftMarker == null) break;
				player.GlobalPosition = baseLevel.LeftMarker.GlobalPosition;
				isMarkerFound = true;
				break;
			default:
				GD.PushError("Invalid entrance for setting player position: " + entrance);
				break;
		}
		if (!isMarkerFound)
			CallDeferred(MethodName.SetPlayerPosition, entrance);
	}
	public Map SearchMap(MapType type)
	{
		GD.Print("Searching for map of type: " + type.ToString());
		List<Map> filteredMaps = Maps.Where(m => m.Type == type && !m.IsEnabled && !m.IsEndLevel && (!m.IsStartLevel || MapPoolIndex == 1)).ToList();
		if (filteredMaps.Count > 0)
		{
			float[] weights = filteredMaps.Select(m => (float)m.RarityWeight).ToArray();
			return Probability.RunWeightedChoose(filteredMaps.ToArray(), weights);
		}
		return null;
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
		GameData.Instance.MapStates.Clear();
		SetPlayerPosition("Start");
	}
	public void ApplyMap()
	{
		var assigned = new Dictionary<Vector2I, Map>();

		foreach (var room in MapALG.Instance.Roomlist)
		{
			GD.Print("Processing room at position: " + room.Position);
			if (!room.IsEnabled) continue;
			MapType desiredType = room.JudgeMapType();
			GD.Print("Desired map type for position " + room.Position + ": " + desiredType.ToString());
			Map chosen = SearchMap(desiredType);
			if (chosen == null)
			{
				GD.PushError("No available map found for type: " + desiredType.ToString());
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
		// foreach (var map in EnabledMaps)
		// {
		// 	GD.Print($"Map at position {map.Position} connections:");
		// 	GD.Print($"  Top: {(map.TopMap != null ? map.TopMap.Position.ToString() : "None")}");
		// 	GD.Print($"  Bottom: {(map.BottomMap != null ? map.BottomMap.Position.ToString() : "None")}");
		// 	GD.Print($"  Left: {(map.LeftMap != null ? map.LeftMap.Position.ToString() : "None")}");
		// 	GD.Print($"  Right: {(map.RightMap != null ? map.RightMap.Position.ToString() : "None")}");
		// }
		Vector2I sp = new(MapALG.Instance.startPos.X, MapALG.Instance.startPos.Y);
		if (assigned.TryGetValue(sp, out var startMap))
		{
			StartMap = startMap;
			StartMap.IsStartLevel = true;
			NowMap = StartMap;
		}
		if (EndNodeMaps.Count == 0)
		{
			GD.PushError("MapManager: No end-node maps found to place the EndLevel. Aborting.");
			return;
		}
		Map nodeToReplace = EndNodeMaps.OrderBy(_ => GD.Randi()).FirstOrDefault();
		EndMap = Maps.Where(map => map.Type == nodeToReplace.Type && map.IsEndLevel).OrderBy(_ => GD.Randi()).FirstOrDefault();
		if (EndMap is null)
		{
			EmitSignal(SignalName.MapGenerated);
			return;
		}
		EndMap.Position = nodeToReplace.Position;
		EndMap.IsEnabled = true;
		if (!EnabledMaps.Contains(EndMap)) EnabledMaps.Add(EndMap);

		nodeToReplace.IsEnabled = false;
		EnabledMaps.Remove(nodeToReplace);

		foreach (var map in EnabledMaps)
		{
			if (map == EndMap) continue;

			if (map.BottomMap == nodeToReplace)
			{
				map.BottomMap = EndMap;
				EndMap.TopMap = map;
				EndMap.TopExit = true;
			}
			if (map.TopMap == nodeToReplace)
			{
				map.TopMap = EndMap;
				EndMap.BottomMap = map;
				EndMap.BottomExit = true;
			}
			if (map.RightMap == nodeToReplace)
			{
				map.RightMap = EndMap;
				EndMap.LeftMap = map;
				EndMap.LeftExit = true;
			}
			if (map.LeftMap == nodeToReplace)
			{
				map.LeftMap = EndMap;
				EndMap.RightMap = map;
				EndMap.RightExit = true;
			}
		}
		EmitSignal(SignalName.MapGenerated);
	}

	public Map GetMapAtPosition(Vector2I position)
	{
		return EnabledMaps.FirstOrDefault(m => m.Position == position);
	}
}
