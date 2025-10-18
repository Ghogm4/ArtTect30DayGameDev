using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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
        InitMaps();
    }
    
    public void OnEntranceChanged(string entrance)
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
            TargetMap = ChooseMap(SortMap(entrance));
            LoadMap(TargetMap, NowMap, entrance);
        }

        if (TargetMap == null)
        {
            GD.Print("No next map found for entrance: " + entrance);
            return;
        }
        SceneManager.Instance.ChangeScene(TargetMap.Scene);
        NowMap = TargetMap;
        GD.Print($"Entrance changed to: {entrance}");
    }

    public void InitMaps()
    {
        foreach (Map map in Maps)
        {
            map.IsEnabled = false;
        }
        Maps = new List<Map>();
        EnabledMaps = new List<Map>();
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
            Maps.Add(map);
        }
    }

    public void StartLevel()
    {
        List<Map> StartMaps = Maps.FindAll(m => m.IsStartLevel);
        List<Map> EndMaps = Maps.FindAll(m => m.IsEndLevel);
        StartMap = ChooseMap(StartMaps);
        EndMap = ChooseMap(EndMaps);
        NowMap = StartMap;
        SceneManager.Instance.ChangeScene(StartMap.Scene);
        StartMap.IsEnabled = true;
        EnabledMaps.Add(StartMap);
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


    public List<Map> SortMap(string entrance)
    {
        List<Map> result = new List<Map>();
        foreach (Map map in Maps)
        {
            if (map.IsEnabled == false)
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
        Tomap.IsEnabled = true;
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
                return map.BottomMap;
            case "Bottom":
                return map.TopMap;
            case "Left":
                return map.RightMap;
            case "Right":
                return map.LeftMap;
            default:
                return null;
        }
    }
}