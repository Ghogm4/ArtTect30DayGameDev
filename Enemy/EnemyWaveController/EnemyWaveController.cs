using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
[GlobalClass]
public partial class EnemyWaveController : Node
{
    [Signal] public delegate void WaveCompletedEventHandler();
    [Signal] public delegate void AllWavesCompletedEventHandler();
    [Export] public float InitialDelay = 0f;
    [Export] public float TimeBetweenWaves = 2f;
    private float _timer = 0f;
    private List<EnemyMap> _enemyMaps = new();
    private int _currentWaveIndex = 0;
    public override void _Ready()
    {
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
            return;
        }
        GetTree().CreateTimer(TimeBetweenWaves).Timeout += StartNextWave;
    }
}
