using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[GlobalClass]
public partial class MapManager : Node
{
	public static MapManager Instance { get; private set; }
	[Export] public Godot.Collections.Array<PackedScene> MapPool;

	public List<Map> Maps = null;
	public List<Map> EnabledMaps = null;

	public string Entrance = null;

	public Map NowMap = null;
	public Map StartMap = null;
	public Map EndMap = null;
	private bool _isEndCreated = false;
	public class Map
	{
		public PackedScene Scene;
		public String SceneFile;
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
		public Map(
			PackedScene scene,
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
			TopExit = topExit;
			BottomExit = bottomExit;
			LeftExit = leftExit;
			RightExit = rightExit;
			IsStartLevel = isStartLevel;
			IsEndLevel = isEndLevel;
			RarityWeight = rarityWeight;
		}
	}

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
		// InitMaps();
		// StartLevel();
	}

	public async void OnEntranceChanged(string entrance)
	{
		Map TargetMap = null;
		Entrance = entrance;
		if (NowMap == null)
		{
			GD.PrintErr("NowMap is null.");
			return;
		}
		else if (NextMap(NowMap, entrance) != null)
		{
			
			TargetMap = NextMap(NowMap, entrance);
		}
		else
		{
			GD.Print(NextMap(NowMap, entrance));
			TargetMap = ChooseMap(SortMap(entrance));
		}
		if (!_isEndCreated && EnabledMaps.Count > 2)
		{
			TargetMap = EndMap;
			_isEndCreated = true;
		}
		if (TargetMap == null)
		{
			GD.Print("TargetMap is null.");
			return;
		}
		LoadMap(TargetMap, NowMap, entrance);
		
		SceneManager.Instance.ChangeScene(TargetMap.Scene);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		SetPlayerPosition(entrance);
		NowMap = TargetMap;
		PrintMap(NowMap);
		
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
		_isEndCreated = false;
		foreach (PackedScene scene in MapPool)
		{
			BaseLevel level = scene.Instantiate<BaseLevel>();
			Map map = new Map(
				scene,
				level.TopExit,
				level.BottomExit,
				level.LeftExit,
				level.RightExit,
				level.IsStartLevel,
				level.IsEndLevel,
				level.RarityWeight);
			if (map.IsEndLevel)
			{
				GD.Print("EndMap assigned.");
				EndMap = map;
			}
			Maps.Add(map);
		}
		GD.Print($"Initialized {Maps.Count} maps.");
	}

	public async Task StartLevel()
	{
		List<Map> StartMaps = Maps.FindAll(m => m.IsStartLevel);
		List<Map> EndMaps = Maps.FindAll(m => m.IsEndLevel);
		StartMap = ChooseMap(StartMaps);
		EndMap = ChooseMap(EndMaps);
		NowMap = StartMap;
		PrintMap(NowMap);
		SceneManager.Instance.ChangeScene(StartMap.Scene);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		Node2D StartMarker = GetTree().CurrentScene.GetNode<Node2D>("%MarkerStart");
		Player player = GetTree().GetNodesInGroup("Player").FirstOrDefault() as Player;
		player.GlobalPosition = StartMarker.GlobalPosition;
		StartMap.IsEnabled = true;
		EnabledMaps.Add(StartMap);
		GD.Print(EnabledMaps.Count);
	}

	public Map ChooseMap(List<Map> maps)
	{
		float total = 0f;
		foreach (Map map in maps)
		{
			total += map.RarityWeight;
		}
		float pick = GD.Randf() * total;
		foreach (Map map in maps)
		{
			pick -= map.RarityWeight;
			if (pick <= 0f)
			{
				return map;
			}
		}
		return null;
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

	public List<Map> SortMap(string entrance)
	{
		List<Map> result = new List<Map>();
		foreach (Map map in Maps)
		{
			if (map.IsEnabled == false && map.IsEndLevel == false)
			{
				switch (entrance)
				{
					case "Top":
						if (map.BottomExit)
						{
							result.Add(map);
						}
						break;
					case "Bottom":
						if (map.TopExit)
						{
							result.Add(map);
						}
						break;
					case "Left":
						if (map.RightExit)
						{
							result.Add(map);
						}
						break;
					case "Right":
						if (map.LeftExit)
						{
							result.Add(map);
						}
						break;
					default:
						return new List<Map> { };
				}
			}
		}
		return result;
	}



	public void LoadMap(Map Tomap, Map Frommap, string entrance)
	{
		GD.Print("here");
		if (!EnabledMaps.Contains(Tomap))
		{
			Tomap.IsEnabled = true;
			EnabledMaps.Add(Tomap);
		}
		switch (entrance)
		{
			case "Top":
				Frommap.TopMap = Tomap;
				Tomap.BottomMap = Frommap;
				break;
			case "Bottom":
				Frommap.BottomMap = Tomap;
				Tomap.TopMap = Frommap;
				break;
			case "Left":
				Frommap.LeftMap = Tomap;
				Tomap.RightMap = Frommap;
				break;
			case "Right":
				Frommap.RightMap = Tomap;
				Tomap.LeftMap = Frommap;
				break;
			default:
				break;
		}

	}

	public Map NextMap(Map map, string entrance)
	{
		switch (entrance)
		{
			case "Top":
				return map.TopMap;
			case "Bottom":
				return map.BottomMap;
			case "Left":
				return map.LeftMap;
			case "Right":
				return map.RightMap;
			default:
				GD.PrintErr("Invalid entrance for NextMap: " + entrance);
				return null;
		}
	}
	
	public void PrintMap(Map map)
	{
		GD.Print($"Map: {map.Scene.ResourcePath}, Top: {map.TopMap}, Bottom: {map.BottomMap}, Left: {map.LeftMap}, Right: {map.RightMap}, IsEnabled: {map.IsEnabled}, IsStartLevel: {map.IsStartLevel}, IsEndLevel: {map.IsEndLevel}, RarityWeight: {map.RarityWeight}");
	}
}
