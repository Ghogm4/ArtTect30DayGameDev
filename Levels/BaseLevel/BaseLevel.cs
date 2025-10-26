using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class BaseLevel : Node2D
{
	[Signal] public delegate void LevelInitializedEventHandler();
    [Export] public bool TopExit = false;
    [Export] public bool BottomExit = false;
    [Export] public bool LeftExit = false;
    [Export] public bool RightExit = false;

    [Export] public Node2D TopMarker;
    [Export] public Node2D BottomMarker;
    [Export] public Node2D LeftMarker;
    [Export] public Node2D RightMarker;
    [Export] public Node2D StartMarker;
    [Export] public Node2D EndMarker;

    [Export] public bool IsStartLevel = false;
    
    [Export] public bool IsEndLevel = false;
    [Export] public Player Player = null;

	[Export] public float RarityWeight = 1.0f;
    public Vector2I MapPosition { get; private set; } = Vector2I.Left;

    public override void _Ready()
    {
        SignalBus.Instance.RegisterSceneChangeStartedAction(OnSceneChangeStarted, SignalBus.Priority.Medium);
    }

    public override void _ExitTree()
    {
        SignalBus.Instance.RemoveSceneChangeStartedAction(OnSceneChangeStarted);
    }

    public void InitializeLevel(Vector2I position)
    {
        MapPosition = position;
		GD.Print($"BaseLevel {MapPosition} initialized.");

		FindAndProcessSavables(this, (savable) =>
		{
			var state = GameData.Instance.LoadObjectState(MapPosition, savable.UniqueID);
			if (state != null)
				savable.LoadState(state);
		});
		EmitSignal(SignalName.LevelInitialized);
    }

    private void OnSceneChangeStarted()
    {
        GD.Print($"BaseLevel {MapPosition}: Saving state...");
        FindAndProcessSavables(this, (savable) => {
            var state = savable.SaveState();
            if (state != null)
            {
                GameData.Instance.SaveObjectState(MapPosition, savable.UniqueID, state);
            }
        });
    }

    private void FindAndProcessSavables(Node startNode, Action<ISavable> action)
    {
        Queue<Node> nodesToProcess = new Queue<Node>();
        nodesToProcess.Enqueue(startNode);

        while (nodesToProcess.Count > 0)
        {
            Node currentNode = nodesToProcess.Dequeue();
            if (currentNode is ISavable savable && currentNode != this)
                action?.Invoke(savable);
        
            foreach (var child in currentNode.GetChildren())
                nodesToProcess.Enqueue(child);
        }
    }
}
