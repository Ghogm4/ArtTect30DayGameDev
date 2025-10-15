using Godot;
using System;

public partial class SceneManager : Node
{
	public static SceneManager Instance { get; private set; }

	public TransitionLayer Transition => GetNode<TransitionLayer>("/root/TransitionLayer");
	public async void ChangeScene(string scenePath)
	{
		await Transition.FadeIn(0.5f);
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.SceneChangeStarted);
		GetTree().ChangeSceneToFile(scenePath);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		await Transition.FadeOut(0.5f);
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// 单例模式
		if (Instance == null)
		{
			Instance = this;
			// 确保切换场景时不被销毁
			ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			QueueFree();
		}
		SignalBus.Instance.PlayerDied += () => ChangeScene("res://UI/StartMenu/StartMenuExclusive/StartMenu.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
