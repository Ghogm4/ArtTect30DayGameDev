using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodotDictionary = Godot.Collections.Dictionary;
[GlobalClass]
public partial class EnemyWaveController : Node, ISavable
{
    public enum LockMode {Entering, FirstWave}
    [Signal] public delegate void WaveCompletedEventHandler();
    [Signal] public delegate void AllWavesCompletedEventHandler();
    [Export] public float InitialDelay = 0f;
    [Export] public float TimeBetweenWaves = 2f;
    [Export] public bool SkipLevelInitialization = false;
    [Export] public LockMode LockWhen = LockMode.Entering;
    [Export] public LevelTransitioner[] LinkedLevelTransitioners = [];
    public string UniqueID => Name;
    private List<EnemyMap> _enemyMaps = new();
    private int _currentWaveIndex = 0;
    private bool _allWavesCompleted = false;
    private void ToggleLevelTransitioners(bool enable)
    {
        foreach (var transitioner in LinkedLevelTransitioners)
            transitioner.CallDeferred(LevelTransitioner.MethodName.ToggleLock, enable);
    }
    public override async void _Ready()
    {
        if (!SkipLevelInitialization)
        {
            BaseLevel baseLevel = GetTree().CurrentScene as BaseLevel;
            if (baseLevel != null)
                await ToSignal(baseLevel, BaseLevel.SignalName.LevelInitialized);
        }
        if (LockWhen == LockMode.Entering)
            ToggleLevelTransitioners(false);
        if (_allWavesCompleted)
        {
            ToggleLevelTransitioners(true);
            return;
        }
        List<EnemyMap> enemyMaps = GetChildren().OfType<EnemyMap>().ToList();
        foreach (EnemyMap enemyMap in enemyMaps)
        {
            _enemyMaps.Add(enemyMap);
            enemyMap.AllEnemiesDefeated += OnAllEnemiesDefeated;
        }
        if (InitialDelay >= 0f)
            GetTree().CreateTimer(InitialDelay).Timeout += () =>
            {
                StartNextWave();
                if (LockWhen == LockMode.FirstWave)
                    ToggleLevelTransitioners(false);
            };
    }

    public void StartNextWave()
    {
        if (_currentWaveIndex == 0 && LockWhen == LockMode.FirstWave)
            ToggleLevelTransitioners(false);
        if (_currentWaveIndex < _enemyMaps.Count)
        {
            _enemyMaps[_currentWaveIndex].ScanMarkers();
            _currentWaveIndex++;
        }
    }
    private void OnAllEnemiesDefeated()
    {
        EmitSignal(SignalName.WaveCompleted);
        if (_currentWaveIndex == _enemyMaps.Count)
        {
            EmitSignal(SignalName.AllWavesCompleted);
            _allWavesCompleted = true;
            ToggleLevelTransitioners(true);
            AudioManager.Instance.PlaySFX("Confirm");
            return;
        }
        GetTree().CreateTimer(TimeBetweenWaves).Timeout += StartNextWave;
    }
    public GodotDictionary SaveState()
    {
        return new()
        {
            ["AllWavesCompleted"] = _allWavesCompleted
        };
    }
	public void LoadState(GodotDictionary state)
	{
		if (state?.TryGetValue("AllWavesCompleted", out var allWavesCompleted) ?? false)
			_allWavesCompleted = (bool)allWavesCompleted;
	}
}
