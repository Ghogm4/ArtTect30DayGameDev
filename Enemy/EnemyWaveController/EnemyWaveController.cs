using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodotDictionary = Godot.Collections.Dictionary;
[GlobalClass]
public partial class EnemyWaveController : Node, ISavable
{
    [Signal] public delegate void WaveCompletedEventHandler();
    [Signal] public delegate void AllWavesCompletedEventHandler();
    [Export] public float InitialDelay = 0f;
    [Export] public float TimeBetweenWaves = 2f;
    [Export] public bool DebugMode = false;
    public string UniqueID => Name;
    private float _timer = 0f;
    private List<EnemyMap> _enemyMaps = new();
    private int _currentWaveIndex = 0;
    private bool _allWavesCompleted = false;
    public override async void _Ready()
    {
        if (!DebugMode)
        {
            BaseLevel baseLevel = GetTree().CurrentScene as BaseLevel;
            if (baseLevel != null)
                await ToSignal(baseLevel, BaseLevel.SignalName.LevelInitialized);
        }
        if (_allWavesCompleted)
            return;
        _timer = InitialDelay;
        List<EnemyMap> enemyMaps = GetChildren().OfType<EnemyMap>().ToList();
        foreach (EnemyMap enemyMap in enemyMaps)
        {
            _enemyMaps.Add(enemyMap);
            enemyMap.AllEnemiesDefeated += OnAllEnemiesDefeated;
        }
        GetTree().CreateTimer(InitialDelay).Timeout += StartNextWave;
    }

    private void StartNextWave()
    {
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
