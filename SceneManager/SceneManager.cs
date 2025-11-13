using Godot;
using System;
using System.Threading.Tasks;
public partial class SceneManager : Node
{
	public static SceneManager Instance { get; private set; }
	public PackedScene StartMenuScene;
	public TransitionLayer Transition => GetNode<TransitionLayer>("/root/TransitionLayer");
	public async void ChangeScene(PackedScene scene)
	{
		await Transition.FadeIn(0.5f);
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.SceneChangeStarted);
		GetTree().ChangeSceneToPacked(scene);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		await Transition.FadeOut(0.5f);
	}
	public async void ChangeScenePath(string scene)
	{
		await Transition.FadeIn(0.5f);
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.SceneChangeStarted);
		GetTree().ChangeSceneToFile(scene);
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
		StartMenuScene = GD.Load<PackedScene>("res://UI/StartMenu/StartMenuExclusive/StartMenu.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
