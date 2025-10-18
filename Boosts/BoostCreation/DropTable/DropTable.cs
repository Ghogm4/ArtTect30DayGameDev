using Godot;
using System;
using System.Collections.Generic;
using Dictionary = Godot.Collections.Dictionary;

[Flags]
public enum BoostRarityFilter
{
    None = 0,
    Common = 1 << 0,
    Uncommon = 1 << 1,
    Rare = 1 << 2,
    Epic = 1 << 3,
    Legendary = 1 << 4,
    All = Common | Uncommon | Rare | Epic | Legendary
}

[Flags]
public enum BoostCategoryFilter
{
    None = 0,
    Combat = 1 << 0,
    Movement = 1 << 1,
    General = 1 << 2,
    All = Combat | Movement | General
}
[GlobalClass]
public partial class DropTable : Node2D
{
    private static HashSet<string> _obtainedOneTimeBoosts = new();

    public static void ResetObtainedOneTimeBoosts()
    {
        _obtainedOneTimeBoosts.Clear();
    }

    [ExportGroup("Drop Settings")]
    [Export] public BoostDropMode DropMode = BoostDropMode.Manual;
    [Export] public int TimesToRun = 1;
    [Export] public bool SkipObtainedOneTimeBoosts = true; // 是否跳过已获得的一次性增益

    [ExportGroup("Boost Sources")]
    [Export] public bool AutoLoadFromDirectories = true;
    [Export] public string[] BoostDirectories = { "res://Boosts/Combat", "res://Boosts/Movement", "res://Boosts/General" };
    [Export] public PackedScene[] ManualBoostList = [];  // Optional manual additions
    [Export] public Dictionary ManualProbabilities = new();  // For manual mode

    private List<PackedScene> _autoLoadedBoosts = new();
    private List<PackedScene> _allBoosts = new();

    [ExportGroup("Rarity Settings")]
    [Export] public bool EnableCommonRarity = true;
    [Export] public bool EnableUncommonRarity = true;
    [Export] public bool EnableRareRarity = true;
    [Export] public bool EnableEpicRarity = true;
    [Export] public bool EnableLegendaryRarity = true;

    [ExportGroup("Category Settings")]
    [Export] public bool EnableCombatItems = true;
    [Export] public bool EnableMovementItems = true;
    [Export] public bool EnableGeneralItems = true;

    private BoostRarityFilter RarityFilter
    {
        get
        {
            var filter = BoostRarityFilter.None;
            if (EnableCommonRarity) filter |= BoostRarityFilter.Common;
            if (EnableUncommonRarity) filter |= BoostRarityFilter.Uncommon;
            if (EnableRareRarity) filter |= BoostRarityFilter.Rare;
            if (EnableEpicRarity) filter |= BoostRarityFilter.Epic;
            if (EnableLegendaryRarity) filter |= BoostRarityFilter.Legendary;
            return filter;
        }
    }

    private BoostCategoryFilter CategoryFilter
    {
        get
        {
            var filter = BoostCategoryFilter.None;
            if (EnableCombatItems) filter |= BoostCategoryFilter.Combat;
            if (EnableMovementItems) filter |= BoostCategoryFilter.Movement;
            if (EnableGeneralItems) filter |= BoostCategoryFilter.General;
            return filter;
        }
    }
    private Dictionary<string, PackedScene> _boostDict = [];
    private Probability _probability = new();
    public override void _Ready()
    {
        LoadBoosts();
        InitDict();
        InitProbability();
        Drop();
    }

    private void LoadBoosts()
    {
        _autoLoadedBoosts.Clear();
        _allBoosts.Clear();

        if (AutoLoadFromDirectories)
        {
            foreach (string directory in BoostDirectories)
            {
                using var dir = DirAccess.Open(directory);
                if (dir == null)
                {
                    GD.PrintErr($"Failed to open directory: {directory}");
                    continue;
                }

                dir.ListDirBegin();
                string fileName = dir.GetNext();
                while (!string.IsNullOrEmpty(fileName))
                {
                    if (!dir.CurrentIsDir() && fileName.EndsWith(".tscn"))
                    {
                        string fullPath = $"{directory}/{fileName}";
                        if (ResourceLoader.Load<PackedScene>(fullPath) is PackedScene scene)
                        {
                            _autoLoadedBoosts.Add(scene);
                        }
                    }
                    fileName = dir.GetNext();
                }
                dir.ListDirEnd();
            }
        }

        // Add manual boosts
        if (ManualBoostList != null)
        {
            foreach (var scene in ManualBoostList)
            {
                if (scene != null)
                {
                    _allBoosts.Add(scene);
                }
            }
        }

        // Add auto-loaded boosts
        _allBoosts.AddRange(_autoLoadedBoosts);
    }

    private void InitDict()
    {
        _boostDict.Clear();
        foreach (var scene in _allBoosts)
        {
            if (scene == null)
                continue;

            string key = scene.ResourcePath.GetFile().GetBaseName();
            _boostDict[key] = scene;
        }
    }

    private bool IsBoostEnabled(Boost boost)
    {
        // 检查一次性增益
        if (SkipObtainedOneTimeBoosts && boost.Info.IsOneTimeOnly)
        {
            string boostPath = boost.SceneFilePath;
            if (!string.IsNullOrEmpty(boostPath) && _obtainedOneTimeBoosts.Contains(boostPath))
                return false;
        }

        bool rarityEnabled = boost.Info.Rarity switch
        {
            BoostRarity.Common => EnableCommonRarity,
            BoostRarity.Uncommon => EnableUncommonRarity,
            BoostRarity.Rare => EnableRareRarity,
            BoostRarity.Epic => EnableEpicRarity,
            BoostRarity.Legendary => EnableLegendaryRarity,
            _ => false
        };

        bool categoryEnabled = boost.Info.Category switch
        {
            BoostCategory.Combat => EnableCombatItems,
            BoostCategory.Movement => EnableMovementItems,
            BoostCategory.General => EnableGeneralItems,
            _ => false
        };

        return rarityEnabled && categoryEnabled;
    }

    private void RegisterBoostSpawn(PackedScene scene, float probability)
    {
        _probability.Register(probability, () =>
        {
            Boost boost = scene.Instantiate<Boost>();
            boost.GlobalPosition = GlobalPosition;

            if (boost.Info.IsOneTimeOnly)
                _obtainedOneTimeBoosts.Add(scene.ResourcePath);

            GetTree().CurrentScene.CallDeferred(MethodName.AddChild, boost);

            float radian = (float)GD.RandRange(-Mathf.Pi * 2 / 3, -Mathf.Pi / 3);
            float force = (float)GD.RandRange(100f, 500f);
            boost.ApplyCentralImpulse(Vector2.Right.Rotated(radian) * force);

        });
    }

    private void InitProbability()
    {
        // Clear existing probability registrations
        _probability = new Probability();

        if (DropMode == BoostDropMode.Manual)
        {
            RegisterManualProbabilities();
        }
        else
        {
            RegisterUniformProbabilities();
        }
    }

    private void RegisterManualProbabilities()
    {
        foreach (var pair in ManualProbabilities)
        {
            string key = pair.Key.AsString();
            float probability = pair.Value.AsSingle();

            if (_boostDict.TryGetValue(key, out PackedScene scene))
            {
                var testBoost = scene.Instantiate<Boost>();
                if (IsBoostEnabled(testBoost))
                {
                    if (SkipObtainedOneTimeBoosts && testBoost.Info.IsOneTimeOnly && _obtainedOneTimeBoosts.Contains(scene.ResourcePath))
                        continue;
                    
                    RegisterBoostSpawn(scene, probability);
                }
                testBoost.QueueFree();
            }
            else
            {
                GD.PushError($"Cannot find the boost named {key}.");
            }
        }
    }

    private void RegisterUniformProbabilities()
    {
        var eligibleBoosts = new List<PackedScene>();

        foreach (var scene in _allBoosts)
        {
            if (scene == null) continue;

            var testBoost = scene.Instantiate<Boost>();
            if (DropMode == BoostDropMode.UniformRarity && IsBoostEnabled(testBoost) ||
                DropMode == BoostDropMode.UniformCategory && IsBoostEnabled(testBoost))
            {
                eligibleBoosts.Add(scene);
            }
            testBoost.QueueFree();
        }

        if (eligibleBoosts.Count > 0)
        {
            float uniformProbability = 1.0f / eligibleBoosts.Count;
            foreach (var scene in eligibleBoosts)
            {
                var testBoost = scene.Instantiate<Boost>();
                if (SkipObtainedOneTimeBoosts && testBoost.Info.IsOneTimeOnly && _obtainedOneTimeBoosts.Contains(scene.ResourcePath))
                {
                    testBoost.QueueFree();
                    continue;
                }
                testBoost.QueueFree();
                RegisterBoostSpawn(scene, uniformProbability);
            }
        }
    }

    private void RegisterBoostProbability(PackedScene scene, float probability)
    {
        if (scene == null)
        {
            GD.PrintErr("DropTable: Attempted to register a null boost scene");
            return;
        }

        var testBoost = scene.Instantiate<Boost>();
        if (SkipObtainedOneTimeBoosts && testBoost.Info.IsOneTimeOnly && _obtainedOneTimeBoosts.Contains(scene.ResourcePath))
        {
            testBoost.QueueFree();
            return;
        }
        testBoost.QueueFree();

        RegisterBoostSpawn(scene, probability);
    }
    public void Drop()
    {
        for (int i = 0; i < TimesToRun; i++)
            _probability.Run();
    }
}
