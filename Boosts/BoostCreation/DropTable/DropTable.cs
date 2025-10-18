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
    [ExportGroup("Drop Settings")]
    [Export] public BoostDropMode DropMode = BoostDropMode.Manual;
    [Export] public int TimesToRun = 1;

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
        // Convert BoostRarity to BoostRarityFilter
        var rarityFilter = boost.Info.Rarity switch
        {
            BoostRarity.Common => BoostRarityFilter.Common,
            BoostRarity.Uncommon => BoostRarityFilter.Uncommon,
            BoostRarity.Rare => BoostRarityFilter.Rare,
            BoostRarity.Epic => BoostRarityFilter.Epic,
            BoostRarity.Legendary => BoostRarityFilter.Legendary,
            _ => BoostRarityFilter.None
        };

        // Convert BoostCategory to BoostCategoryFilter
        var categoryFilter = boost.Info.Category switch
        {
            BoostCategory.Combat => BoostCategoryFilter.Combat,
            BoostCategory.Movement => BoostCategoryFilter.Movement,
            BoostCategory.General => BoostCategoryFilter.General,
            _ => BoostCategoryFilter.None
        };

        // Check if both the rarity and category are enabled in their respective filters
        return RarityFilter.HasFlag(rarityFilter) && CategoryFilter.HasFlag(categoryFilter);
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
                    RegisterBoostProbability(scene, probability);
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
                RegisterBoostProbability(scene, uniformProbability);
            }
        }
    }

    private void RegisterBoostProbability(PackedScene scene, float probability)
    {
        _probability.Register(probability, () =>
        {
            Boost boost = scene.Instantiate<Boost>();
            boost.GlobalPosition = GlobalPosition;
            Scheduler.Instance.ScheduleAction(2f, () =>
            {
                GetTree().CurrentScene.CallDeferred(MethodName.AddChild, boost);

                float radian = (float)GD.RandRange(-Mathf.Pi * 2 / 3, -Mathf.Pi / 3);
                float force = (float)GD.RandRange(100f, 500f);
                boost.ApplyCentralImpulse(Vector2.Right.Rotated(radian) * force);
            }, 0);
        });
    }
    public void Drop()
    {
        for (int i = 0; i < TimesToRun; i++)
            _probability.Run();
    }
}
