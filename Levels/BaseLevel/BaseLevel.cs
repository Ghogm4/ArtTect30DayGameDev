using Godot;
using System;
using System.Collections;
using System.Collections.Generic; // Make sure this is included

public partial class BaseLevel : Node2D
{
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
    public Vector2I MapPosition = Vector2I.Left;
    public override void _Ready()
    {
        SignalBus.Instance.RegisterSceneChangeStartedAction(OnSceneChangeStarted, SignalBus.Priority.Medium);
        MapPosition = MapManager.Instance.NowMap.Position;
        GD.Print("BaseLevel _Ready: Loading state for level at position " + MapPosition);
        
        FindAndProcessSavables(this, (savable) => {
            savable.LoadState(GameData.Instance.LoadObjectState(MapPosition, savable.UniqueID));
        });
    }

    public override void _ExitTree()
    {
        SignalBus.Instance.RemoveSceneChangeStartedAction(OnSceneChangeStarted);
    }

    private void OnSceneChangeStarted()
    {
        GD.Print("BaseLevel OnSceneChangeStarted: Saving state for level at position " + MapPosition);
		FindAndProcessSavables(this, (savable) =>
		{
			GameData.Instance.SaveObjectState(MapPosition, savable.UniqueID, savable.SaveState());
		});
		SignalBus.Instance.RemoveSceneChangeStartedAction(OnSceneChangeStarted);
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
