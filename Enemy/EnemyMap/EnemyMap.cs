using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class EnemyMap : Node
{
	[Export] public PackedScene[] EnemyList = Array.Empty<PackedScene>();
	private Dictionary<string, PackedScene> EnemyDict = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitDict();
		ScanMarkers();
		
	}
	
	public void InitDict()
	{
		foreach (var scene in EnemyList)
		{
			if (scene != null)
			{
				string key = scene.ResourcePath.GetFile().GetBaseName();
				EnemyDict[key] = scene;
				GD.Print($"Loaded Enemy: {key}");
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ScanMarkers()
	{
		foreach (Node child in GetChildren())
		{
			if (child is EnemyMarker marker)
			{
				SpawnMarker(marker);
			}
		}
	}

	public void SpawnMarker(EnemyMarker marker)
	{
		Vector2 position = marker.GlobalPosition;
		Probability probability = new();
		foreach (string enemyName in marker.EnemyTypes.Keys)
		{
			probability.Register(marker.EnemyTypes[enemyName], () => SpawnEnemy(enemyName, position));
		}
		probability.Run();
	}

	public void SpawnEnemy(string enemyName, Vector2 position)
	{
		if (EnemyDict.ContainsKey(enemyName))
		{
			var enemy = EnemyDict[enemyName];
			var enemyInstance = enemy.Instantiate<Node2D>();
			enemyInstance.Position = position;
			GetTree().CurrentScene.CallDeferred("add_child", enemyInstance);
		}
		else
		{
			GD.PrintErr($"Enemy '{enemyName}' not found in EnemyDict.");
		}
	}
}
