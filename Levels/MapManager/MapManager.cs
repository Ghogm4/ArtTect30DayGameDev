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
            GD.Print("No next map found for entrance: " + entrance);
            return;
        }
        SceneManager.Instance.ChangeScene(TargetMap.Scene);
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
        foreach (Map map in Maps)
        {
            if (map.IsStartLevel)
            {
                SceneManager.Instance.ChangeScene(map.Scene);
                return;
            }
        }
    }

    public Map ChooseMap(List<Map> maps)
    {
        foreach (Map map in maps)
        {

        }
        return null;
    }

    public void LoadMap(Map Tomap, Map Frommap)
    {
        Tomap.IsEnabled = true;
        if (!EnabledMaps.Contains(Tomap))
        {
            Tomap.IsEnabled = true;
            EnabledMaps.Add(Tomap);
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