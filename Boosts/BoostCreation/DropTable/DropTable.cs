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
    [ExportGroup("Default Drops")]
    [Export] public PackedScene[] DefaultDropScenes;

    private static readonly Dictionary<BoostRarity, float> _baseRarityWeights = new()
    {
        { BoostRarity.Common, 0.60f },
        { BoostRarity.Uncommon, 0.23f },
        { BoostRarity.Rare, 0.10f },
        { BoostRarity.Epic, 0.05f },
        { BoostRarity.Legendary, 0.02f }
    };

    private static HashSet<string> _obtainedOneTimeBoosts = new();

    public static void ResetObtainedOneTimeBoosts() => _obtainedOneTimeBoosts.Clear();
    
    [ExportGroup("Drop Settings")]
    [Export] public BoostDropMode DropMode = BoostDropMode.UniformRarity;
    [Export] public int TimesToRun = 1;
    [Export] public bool SkipObtainedOneTimeBoosts = true;

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
    }

    private void LoadBoosts()
    {
        _autoLoadedBoosts.Clear();
        _allBoosts.Clear();

        DoAutoLoadFromDirectories();

        DoManualBoostLoading();

        _allBoosts.AddRange(_autoLoadedBoosts);
    }
    private void DoAutoLoadFromDirectories()
    {
        if (!AutoLoadFromDirectories)
            return;

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
                        _autoLoadedBoosts.Add(scene);
                }
                fileName = dir.GetNext();
            }
            dir.ListDirEnd();
        }
    }
    private void DoManualBoostLoading()
    {
        if (ManualBoostList == null || ManualBoostList.Length == 0)
            return;

        foreach (var scene in ManualBoostList)
            if (scene != null)
                _allBoosts.Add(scene);
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
    private void InitProbability()
    {
        _probability = new Probability();

        if (DropMode == BoostDropMode.Manual)
            RegisterManualProbabilities();
        else
            RegisterUniformProbabilities();
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
        if (DropMode == BoostDropMode.UniformRarity)
            RegisterRarityBasedProbabilities();
        else if (DropMode == BoostDropMode.UniformCategory)
            RegisterCategoryBasedProbabilities();
    }
    private bool IsBoostEnabled(Boost boost)
    {
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
            if (SkipObtainedOneTimeBoosts)
            {
                var testBoost = scene.Instantiate<Boost>();
                bool isOneTimeOnly = testBoost.Info.IsOneTimeOnly;
                testBoost.QueueFree();

                if (isOneTimeOnly)
                    _obtainedOneTimeBoosts.Add(scene.ResourcePath);
            }
            AddBoostFromPackedSceneToLevel(scene);
        });
    }

    private void RegisterCategoryBasedProbabilities()
    {
        var eligibleBoosts = new List<PackedScene>();

        foreach (var scene in _allBoosts)
        {
            if (scene == null) continue;

            var testBoost = scene.Instantiate<Boost>();
            if (IsBoostEnabled(testBoost))
            {
                if (!(SkipObtainedOneTimeBoosts && testBoost.Info.IsOneTimeOnly &&
                    _obtainedOneTimeBoosts.Contains(scene.ResourcePath)))
                    eligibleBoosts.Add(scene);
            }
            testBoost.QueueFree();
        }

        if (eligibleBoosts.Count > 0)
        {
            float uniformWeight = 1.0f;
            foreach (var scene in eligibleBoosts)
                RegisterBoostSpawn(scene, uniformWeight);
        }
    }

    private void RegisterRarityBasedProbabilities()
    {
        var boostsByRarity = new Dictionary<BoostRarity, List<PackedScene>>();

        // 收集每个稀有度的可用Boost
        foreach (var scene in _allBoosts)
        {
            if (scene == null) continue;

            var testBoost = scene.Instantiate<Boost>();
            if (!IsBoostEnabled(testBoost))
            {
                testBoost.QueueFree();
                continue;
            }

            var rarity = testBoost.Info.Rarity;
            if (!boostsByRarity.ContainsKey(rarity))
                boostsByRarity[rarity] = new List<PackedScene>();
            
            if (!(SkipObtainedOneTimeBoosts && testBoost.Info.IsOneTimeOnly &&
                _obtainedOneTimeBoosts.Contains(scene.ResourcePath)))
                boostsByRarity[rarity].Add(scene);

            testBoost.QueueFree();
        }

        // 注册每个稀有度的Boosts
        foreach (var rarityGroup in boostsByRarity)
        {
            if (rarityGroup.Value.Count == 0) continue;
            if (!_baseRarityWeights.TryGetValue(rarityGroup.Key, out float baseWeight)) continue;

            // 在该稀有度内平均分配权重
            float weight = baseWeight;
            foreach (var scene in rarityGroup.Value)
                RegisterBoostSpawn(scene, weight / rarityGroup.Value.Count);
        }
    }

    private void RegisterBoostProbability(PackedScene scene, float weight)
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

        RegisterBoostSpawn(scene, weight);
    }

    private void SpawnDefaultDrop()
    {
        if (DefaultDropScenes == null || DefaultDropScenes.Length == 0)
        {
            GD.PushError("DropTable: Default drops are not configured!");
            return;
        }

        AddBoostFromPackedSceneToLevel(GetRandomDefaultDropScene());
    }
    private PackedScene GetRandomDefaultDropScene() => Probability.RunUniformChoose(DefaultDropScenes);
    private void AddBoostFromPackedSceneToLevel(PackedScene scene)
    {
        if (scene == null)
        {
            GD.PushError("DropTable: Attempted to add a null boost scene to the scene tree.");
            return;
        }

        var boost = scene.Instantiate<Boost>();
        boost.GlobalPosition = GlobalPosition;

        GetTree().CurrentScene.CallDeferred(MethodName.AddChild, boost);

        float spread = Mathf.Atan(TimesToRun) / 2;
        float radian = (float)GD.RandRange(-Mathf.Pi / 2 - spread, -Mathf.Pi / 2 + spread);
        float force = 200f;
        boost.ApplyCentralImpulse(Vector2.Right.Rotated(radian) * force);
    }
    private bool HasValidDrops()
    {
        foreach (var scene in _allBoosts)
        {
            if (scene == null) continue;

            var testBoost = scene.Instantiate<Boost>();
            bool isValid = IsBoostEnabled(testBoost);

            if (isValid && testBoost.Info.IsOneTimeOnly &&
                _obtainedOneTimeBoosts.Contains(scene.ResourcePath))
                isValid = false;
            
            testBoost.QueueFree();

            if (isValid)
                return true;
        }
        return false;
    }

    private bool TryRunProbability()
    {
        bool success = false;
        _probability.Register(1.0f, () => success = true);
        _probability.Run();
        return success;
    }

    public void Drop()
    {
        int remainingDrops = TimesToRun;
        int maxAttempts = TimesToRun * 3;
        int attempts = 0;

        while (remainingDrops > 0 && attempts < maxAttempts)
        {
            attempts++;

            if (!HasValidDrops())
            {
                SpawnDefaultDrop();
                remainingDrops--;
                continue;
            }

            InitProbability();
            _probability.Run();
            if (!_probability.IsEmpty)
                remainingDrops--;
        }
    }
}
