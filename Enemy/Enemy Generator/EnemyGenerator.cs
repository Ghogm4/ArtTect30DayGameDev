using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class EnemyGenerator : Node
{
    public static EnemyGenerator Instance { get; private set; }
    public Dictionary<string, PackedScene> EnemyDict = new Dictionary<string, PackedScene>();

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
    }

    public void LoadEnemy(string name, string path)
    {
        var scene = GD.Load<PackedScene>(path);
        if (scene != null)
        {
            EnemyDict[name] = scene;
        }
    }

    public async void SummonEnemy(string name, Vector2 position)
    {
        if (EnemyDict.ContainsKey(name))
        {
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
            var enemyInstance = EnemyDict[name].Instantiate<Node2D>();
            enemyInstance.Position = position;
            GetTree().CurrentScene.AddChild(enemyInstance);
        }
        else
        {
            GD.PrintErr($"Enemy '{name}' not found in EnemyDict.");
        }
    }
}
