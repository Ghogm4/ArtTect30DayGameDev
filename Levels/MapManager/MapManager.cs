using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class MapManager : Node2D
{
	public static MapManager Instance { get; private set; }
	[Export] public Godot.Collections.Array<PackedScene> MapPool;
	[Signal] public delegate void MapGeneratedEventHandler();
	[Signal] public delegate void MapChangedEventHandler();
	public enum MapType
	{
		T, B, L, R, TB, TL, TR, BL, BR, LR, TBL, TBR, TLR, BLR, TBLR
	}
	public class Map
	{
		public PackedScene Scene;
		public Tuple<int, int> Position;
		public bool TopExit;
		public bool BottomExit;
		public bool LeftExit;
		public bool RightExit;

		public bool IsEnabled = false;
		public bool IsStartLevel = false;
		public bool IsEndLevel = false;

		public float RarityWeight = 1.0f;

		public Map LeftMap = null;
		public Map RightMap = null;
		public Map TopMap = null;
		public Map BottomMap = null;
		public MapType Type;
		public bool IsDiscovered = false;
		public Map(
			PackedScene scene,
			Tuple<int, int> position,
			bool topExit,
			bool bottomExit,
			bool leftExit,
			bool rightExit,
			bool isStartLevel = false,
			bool isEndLevel = false,
			float rarityWeight = 1.0f
			)
		{
			Scene = scene;
			Position = position;
			TopExit = topExit;
			BottomExit = bottomExit;
			LeftExit = leftExit;
			RightExit = rightExit;
			IsStartLevel = isStartLevel;
			IsEndLevel = isEndLevel;
			RarityWeight = rarityWeight;
			Type = this.JudgeType();
		}
		public Map GetMap(string entrance)
		{
			return entrance switch
			{
				"Top" => TopMap,
				"Bottom" => BottomMap,
				"Left" => LeftMap,
				"Right" => RightMap,
				_ => null,
			};
		}
		public MapType JudgeType()
		{
			if (TopExit && !BottomExit && !LeftExit && !RightExit)
			{
				Type = MapType.T;
			}
			else if (!TopExit && BottomExit && !LeftExit && !RightExit)
			{
				Type = MapType.B;
			}
			else if (!TopExit && !BottomExit && LeftExit && !RightExit)
			{
				Type = MapType.L;
			}
			else if (!TopExit && !BottomExit && !LeftExit && RightExit)
			{
				Type = MapType.R;
			}
			else if (TopExit && BottomExit && !LeftExit && !RightExit)
			{
				Type = MapType.TB;
			}
			else if (TopExit && !BottomExit && LeftExit && !RightExit)
			{
				Type = MapType.TL;
			}
			else if (TopExit && !BottomExit && !LeftExit && RightExit)
			{
				Type = MapType.TR;
			}
			else if (!TopExit && BottomExit && LeftExit && !RightExit)
			{
				Type = MapType.BL;
			}
			else if (!TopExit && BottomExit && !LeftExit && RightExit)
			{
				Type = MapType.BR;
			}
			else if (!TopExit && !BottomExit && LeftExit && RightExit)
			{
				Type = MapType.LR;
			}
			else if (TopExit && BottomExit && LeftExit && !RightExit)
			{
				Type = MapType.TBL;
			}
			else if (TopExit && BottomExit && !LeftExit && RightExit)
			{
				Type = MapType.TBR;
			}
			else if (TopExit && !BottomExit && LeftExit && RightExit)
			{
				Type = MapType.TLR;
			}
			else if (!TopExit && BottomExit && LeftExit && RightExit)
			{
				Type = MapType.BLR;
			}
			else if (TopExit && BottomExit && LeftExit && RightExit)
			{
				Type = MapType.TBLR;
			}
			return Type;
		}
		public string GetMapTypeString()
		{
			return Type.ToString();
		}
	}
	public List<Map> Maps = null;
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

		SignalBus.Instance.EntranceSignal += OnEntranceChanged;
	}


	public async void OnEntranceChanged(string entrance)
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
		SetPlayerPosition(entrance);
		NowMap = TargetMap;
		if (!NowMap.IsDiscovered)
		{
			NowMap.IsDiscovered = true;
		}
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
			CallDeferred(nameof(SetPlayerPosition), entrance);
			return;
		}

		switch (entrance)
		{
			case "Top":
				if (baseLevel.BottomMarker == null) { CallDeferred(nameof(SetPlayerPosition), entrance); return; }
				player.GlobalPosition = baseLevel.BottomMarker.GlobalPosition;
				break;
			case "Bottom":
				if (baseLevel.TopMarker == null) { CallDeferred(nameof(SetPlayerPosition), entrance); return; }
				player.GlobalPosition = baseLevel.TopMarker.GlobalPosition;
				break;
			case "Left":
				if (baseLevel.RightMarker == null) { CallDeferred(nameof(SetPlayerPosition), entrance); return; }
				player.GlobalPosition = baseLevel.RightMarker.GlobalPosition;
				break;
			case "Right":
				if (baseLevel.LeftMarker == null) { CallDeferred(nameof(SetPlayerPosition), entrance); return; }
				player.GlobalPosition = baseLevel.LeftMarker.GlobalPosition;
				break;
			case "Start":
				if (baseLevel.StartMarker == null) { CallDeferred(nameof(SetPlayerPosition), entrance); return; }
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
		if (Maps != null)
		{
			foreach (Map map in Maps)
			{
				map.IsEnabled = false;
			}
		}
		Maps = new List<Map>();
		EnabledMaps = new List<Map>();
		foreach (PackedScene scene in MapPool)
		{
			var mapLevel = scene.Instantiate() as BaseLevel;
			if (mapLevel == null)
			{
				GD.PrintErr("MapLevel is null.");
				continue;
			}
			Map newMap = new Map(
				scene,
				null,
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
			{
				StartMap = newMap;
			}
			if (newMap.IsEndLevel)
			{
				EndMap = newMap;
			}
			GD.Print("Loaded map: " + scene.ResourcePath);
		}

		MapALG.Instance.InitMap();
		MapALG.Instance.StartRoom();
		MapALG.Instance.PrintMap();
		ApplyMap();
		EmitSignal(SignalName.MapGenerated);
	}
	public async Task StartLevel()
	{
		NowMap = StartMap;
		StartMap.IsDiscovered = true;
		EmitSignal(SignalName.MapChanged);
		SceneManager.Instance.ChangeScene(NowMap.Scene);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		SetPlayerPosition("Start");
	}
	public void ApplyMap()
	{
		var assigned = new Dictionary<Tuple<int, int>, Map>();
		var rng = new Random();

		foreach (var room in MapALG.Instance.Roomlist)
		{
			if (!room.IsEnabled) continue;
			var roomTypeName = room.JudgeMapType().ToString();
			MapType desiredType = (MapType)Enum.Parse(typeof(MapType), roomTypeName);
			Map chosen = SearchMap(desiredType);
			if (chosen == null)
			{
				GD.PrintErr("No available map found for type: " + roomTypeName);
				continue;
			}
			if (room.Position.Item1 == (int)MapALG.Instance.startPos.X && room.Position.Item2 == (int)MapALG.Instance.startPos.Y)
			{
				chosen = StartMap;
			}
			chosen.IsEnabled = true;
			chosen.Position = room.Position;
			assigned[room.Position] = chosen;
			EnabledMaps.Add(chosen);
		}

		foreach (var kv in assigned)
		{
			var room = MapALG.Instance.Get(kv.Key.Item1, kv.Key.Item2);
			var map = kv.Value;
			if (room.TopExit)
			{
				var key = new Tuple<int, int>(room.Position.Item1, room.Position.Item2 - 1);
				if (assigned.ContainsKey(key))
				{
					map.TopMap = assigned[key];
					assigned[key].BottomMap = map;
					GD.Print("Connected", map.Position, "top to", assigned[key].Position);
				}
			}
			if (room.BottomExit)
			{
				var key = new Tuple<int, int>(room.Position.Item1, room.Position.Item2 + 1);
				if (assigned.ContainsKey(key))
				{
					map.BottomMap = assigned[key];
					assigned[key].TopMap = map;
					GD.Print("Connected", map.Position, "bottom to", assigned[key].Position);
				}
			}
			if (room.LeftExit)
			{
				var key = new Tuple<int, int>(room.Position.Item1 - 1, room.Position.Item2);
				if (assigned.ContainsKey(key))
				{
					map.LeftMap = assigned[key];
					assigned[key].RightMap = map;
					GD.Print("Connected", map.Position, "left to", assigned[key].Position);
				}
			}
			if (room.RightExit)
			{
				var key = new Tuple<int, int>(room.Position.Item1 + 1, room.Position.Item2);
				if (assigned.ContainsKey(key))
				{
					map.RightMap = assigned[key];
					assigned[key].LeftMap = map;
					GD.Print("Connected", map.Position, "right to", assigned[key].Position);
				}
			}

			if (room.getValid() == 1 && !EndNodeMaps.Contains(map) && map != StartMap)
			{
				EndNodeMaps.Add(map);
			}
		}
		var sp = new Tuple<int, int>((int)MapALG.Instance.startPos.X, (int)MapALG.Instance.startPos.Y);
		if (assigned.TryGetValue(sp, out var sMap))
		{
			StartMap = sMap;
			StartMap.IsStartLevel = true;
			NowMap = StartMap;
		}

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
	
	public Map GetMapAtPosition(Tuple<int, int> position)
	{
		return EnabledMaps.FirstOrDefault(m => m.Position != null && m.Position.Item1 == position.Item1 && m.Position.Item2 == position.Item2);
	}
}
